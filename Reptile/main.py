import argparse
import configparser
import urllib3
import re
import os
import time
import random
import math
import sys

# 设置命令行参数解析
parser = argparse.ArgumentParser(description='网页内容爬取脚本。')
parser.add_argument('-C','--config', type=str, default='main.ini', help='配置文件路径。')
parser.add_argument('-U','--update', action='store_true', help='更新配置文件的标志。')
parser.add_argument('-T', '--test', action='store_true', help='执行测试模块')
parser.add_argument('-P', '--path', action='store_true', help='指定测试文件路径')
parser.add_argument('-V','--value', action='store_true', help='指定测试章节内容')
parser.add_argument('-O','--out',action='store_true',help='获取章节内容')
parser.add_argument('-I','--index',type=int,default=0,help='章节指针')
parser.add_argument('-R','--rule',action='store_true',help='规则指定')
parser.add_argument('-DI','--readDDIndex',type=int,default=6,help='指定使用第几条读取章节ID的规则')
parser.add_argument('-TI','--readTextIndex',type=int,default=4,help='指定使用第几条读取文本内容的规则')
#parser.add_argument('-HT','--haveTitle',action='store_true',help='是否有章节头部(类似于第xx章),用于小说阅读器')
args = parser.parse_args()

# 读取配置文件
config = configparser.ConfigParser()
config.read(args.config)

# 修改INI
# 检查是否提供了 -R 参数和 -DI 参数
if args.rule:
    # 确保 readDDIndex 参数也被提供了
    if args.readDDIndex is not None:
        # 更新配置文件中的 readDD 值
        config['DEFAULT']['readDD'] = str(args.readDDIndex)

        # 将更新写回配置文件
        with open(args.config, 'w') as configfile:
            config.write(configfile)
        #print(f'配置已更新，readDD 设置为 {args.readDDIndex}。')
        #sys.exit()  # 更新完成后退出程序

    elif args.readTextIndex is not None:
        # 更新配置文件中的 readDD 值
        config['DEFAULT']['readText'] = str(args.readTextIndex)

        # 将更新写回配置文件
        with open(args.config, 'w') as configfile:
            config.write(configfile)
        #print(f'配置已更新，readText 设置为 {args.readTextIndex}。')
        #sys.exit()  # 更新完成后退出程序


# 如果存在命令行参数用于更新配置，更新并保存配置文件
if args.update:
    config['DEFAULT']['webUrl'] = input('请输入爬取网址：')
    config['DEFAULT']['webUrlForEach'] = input('请输入截断网址：')
    # 可以继续为其他配置项添加相应的输入更新逻辑

    with open(args.config, 'w') as configfile:
        config.write(configfile)
    print('配置已更新。')
    exit()

# 使用配置项
webUrl = config['DEFAULT'].get('webUrl', '')
webUrlForEach = config['DEFAULT'].get('webUrlForEach', '')
file = config['DEFAULT'].get('file', 'output.txt')
ini = config['DEFAULT'].get('ini', 'output.ini')
start = config['DEFAULT'].getint('start', 10)  # 默认值为11，因为原来是10 + 1
passUrl = config['DEFAULT'].get('passUrl', '')
passName = config['DEFAULT'].get('passName', '无标题章节')
needProxy = config['DEFAULT'].getboolean('needProxy', False)
needVerify = config['DEFAULT'].getboolean('needVerify', True)
ignoreDecode = config['DEFAULT'].getboolean('ignoreDecode', False)
isLines = config['DEFAULT'].getboolean('isLines', False)
haveTitle = config['DEFAULT'].getboolean('haveTitle', False)
timeWait = config['DEFAULT'].get('timeWait', '1,3').split(',')  # 转换为列表
timeWait = [int(time) for time in timeWait]  # 转换为整数列表
maxErrorTimes = config['DEFAULT'].getint('maxErrorTimes', 10)
removeHTML = config['DEFAULT'].getboolean('removeHTML', False)
endText = config['DEFAULT'].get("endText","___END___")

#----------------------------------------------------------#
# 爬虫头
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36' }
# 章节读取器
# readDD = re.compile(config['RULE'].get('readDD6',r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>'))
readDDS = [];
for key in config['RULE']:
    pattern = config['RULE'][key]
    readDDS.append(re.compile(pattern))
readDD = readDDS[config['DEFAULT'].getint('readDD')];
if readDD == "":
    readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
else:
    readDD = re.compile(readDD);

readTexts = [];
if isLines:
    for key in config['TEXTRULE']:
        readTexts.append(config['TEXTRULE'][key]);
else:
    for key in config['SECTIONRULE']:        
        readTexts.append(config['SECTIONRULE'][key]);
readText = readTexts[config['DEFAULT'].getint('readText')];

# 随机数生成器
r = random.Random();
iniCount = 4;                               #ini行数
errorTimes = 0;                             #错误次数
if needProxy:                               #设置代理(小飞机)
    os.environ["http_proxy"] = config['STATIC'].get("proxyPath");
    os.environ["https_proxy"] = config['STATIC'].get("proxyPath");

if needVerify==False:
    urllib3.disable_warnings();

#os.environ["TERM"] = "ansi";                #设置环境变量用于显示有色终端内容(无效)
file_path = os.path.abspath(__file__)
dir_path = os.path.dirname(file_path)       #当前文件所在文件夹
#reptileGet = True;                          #爬取成功
# 原始代码中的替换操作可以通过创建一个替换字典来简化
replacements = {
    "&nbsp;": " ",
    "<br /><br />": "\n",
    "<br/><br/>": "\n",
    "<br><br>": "\n",
    "<br />": "\n",
    "<br/>": "\n",
    "<br>": "\n",
    "<p>": "",
    "</p>": "\n",
    "\t":"",
    "    ":" ",
    #"\n\n": "\n",  # 可能需要额外的逻辑来处理连续的换行
    "\n \n": "\n",
    "\n    \n": "\n",
    "</div>": "\n",
    "&ldquo;": "\"",
    "&lsquo;": "'",
    "&rsquo;": "'",
    "&rdquo;": "\"",
    "&hellip;": "…",
    "&mdash;": "—",
    "&amp;": "&",
    "&lt;": "<",
    "&gt;": ">",
    "澹": "淡"
}
#----------------------------------------------------------#

try:
    # 实例化产生请求对象
    
    if needProxy:
        #proxy = {
        #    'http':'127.0.0.1:33210',    
        #    'https':'127.0.0.1:33210',    
        #};
        #http = urllib3.ProxyManager(proxy,headers = headers);
        http = urllib3.ProxyManager("http://127.0.0.1:33210",headers = headers,cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"));
    else:
        http = urllib3.PoolManager(cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"))

    # get请求指定网址
    #res = http.request("GET",webUrl)
    res = http.request("GET",webUrl,None,headers);

    #res = http.request(
    #   "GET",
    #    url,
    #    headers={
    #        'User-Agent':'Mozilla/5.0(WindowsNT6.1;rv:2.0.1)Gecko/20100101Firefox/4.0.1',
    #    },
    #    fields={'id':100,'name':'lisi'}, #请求参数信息
    #)

    # 获取HTTP状态码
    print("status:%d" % res.status)

    try:
        # 获取响应内容
        data = res.data.decode("utf-8")
            
    except UnicodeDecodeError as err:
        data = res.data.decode("gbk");

    #data.replace("\<!\>","")              #删除特殊注释
    allDD = readDD.findall(data);
    allDD = allDD[start:]              #消除初始推荐章节
    
    #测试路径爬取是否正确
    if args.test:
        if args.path:
            print(allDD[:10]);
            print(endText);
            sys.exit();

    urladds = [];
    names = [];
    i = 0;

    #判断INI是否存在
    try:
        lines = openReadLines(ini);
        if len(lines)!=iniCount:
            for x in allDD:
                #y = x[0].split("/")
                #urladds.append(y[len(y)-1]);
                #urladds.append(x[0]);
                #y = x[1].replace("\n","");
                urladds.append(x[1]);
                y = x[2].replace("\n","");
                y = y.replace("  ","");
                y = y.replace(",","，"); #防止章节名称中出现逗号导致读取分割失败
                names.append(y);
            saveIni(webUrl,urladds,names,0);
        else:
            x = lines[1]
            x = x.split(":");
            x = x[1]
            urladds = x.split(",");
            x = lines[2]
            x = x.split(":");
            x = x[1]
            names = x.split(",");
            x = lines[3];
            x = x.split(":");
            i = int(x[1]);
            #判断一下ini文件和网站读取的数据是否相符合
            oldCount = len(urladds);
            newCount = len(allDD);
            if oldCount!=newCount:
                for j in range(oldCount,newCount):
                    x = allDD[j];
                    urladds.append(x[1]);
                    y = x[2].replace("\n","");
                    y = y.replace("  ","");
                    y = y.replace(",","，"); #防止章节名称中出现逗号导致读取分割失败
                    y = y.replace(":","："); #防止章节名称中出现冒号导致读取分割失败
                    names.append(y);
                saveIni(webUrl,urladds,names,i);#刷新并保存ini
    except:
        for x in allDD:
            #y = x[0].split("/")
            #urladds.append(y[len(y)-1]);
            #urladds.append(x[0]);
            #y = x[1].replace("\n","");
            urladds.append(x[1]);
            y = x[2].replace("\n","");
            y = y.replace("  ","");
            y = y.replace(",","，"); #防止章节名称中出现逗号导致读取分割失败
            y = y.replace(":","："); #防止章节名称中出现冒号导致读取分割失败
            names.append(y);
        saveIni(webUrl,urladds,names,0);

    #urladds = urladds[i:]             #跳过已有章节
    #names = names[i:]             #跳过已有章节
    need = len(urladds);
    pageCount = len(allDD);         #总章节数量

    while(i<len(names)):
        
        #以下是读取P行的代码

        #x = urladds[j];
        #y = names[j];
        #url = webUrlForEach+x;
        #res = http.request("GET",url);
        ##print(res.status);
        #try:
        #    eachData = res.data.decode("utf-8");
        #except UnicodeDecodeError as err2:
        #    eachData = res.data.decode("gbk");

        ##print(eachData);
        
        ##text = re.compile(r'<p class=".*">([^<>]*)<\/p>')
        #text = re.compile(r'<p>([^<>]*)<\/p>')
        
        #allText = text.findall(eachData);
        ##allText = allText[0];
        ##allText = allText.replace("&nbsp;"," ");
        ##allText = allText.replace("<br /><br />","\n");
        ##openWriteAdd(allText);
        #openWriteAdd("\r\n");
        #openWriteAdd(y);
        #openWriteAdd("\r\n");
        ##openWrites(allText);
        #openWrites(allText[:len(allText)-3]);       #去掉最后行尾网站信息
        #print("第"+str(i)+"章已经下载完成");
        #i+=1;
        #changeIniIndex(i);
        #time.sleep(r.randint(3,7));

        
        x = urladds[i];
        y = names[i];
        url = webUrlForEach+x;
        if url == webUrlForEach+passUrl or y==passName:
            i+=1;
            continue;
        res = http.request("GET",url,None,headers);
        #print(res.status);
        try:
            eachData = res.data.decode("utf-8");
        except UnicodeDecodeError as err2:
            eachData = res.data.decode("gbk",errors= (ignoreDecode==False and 'replace'or'ignore'));

        #print(eachData);
        if args.test:
            if args.value:
                print(eachData);
                print(endText);
                sys.exit();
        
        text = re.compile(readText);
        
        #eachData = eachData.replace("\x3C","<");    #修复特殊字符

        allText = text.findall(eachData);

        if isLines == False:
            try:
                allText = allText[0];
                errorTimes = 0;
            except IndexError:
                #休眠一次时间后重试
                errorTimes +=1;
                if errorTimes==1:
                    print("当前章节指针:"+str(i),"章节名称:",y,"\n","章节网址:",url);
                consoleWrite("[error] ","red");
                print("爬取失败,等待重试中,重试次数:"+str(errorTimes));
                if errorTimes>maxErrorTimes:
                    raise IndexError("爬取第"+str(i+1)+"章节失败.\n章节名称:"+y+"\n章节网址:\n"+url+"\n");
                time.sleep(r.randint(timeWait[0],timeWait[1]));
                continue;
            #allText = allText.replace("&nbsp;"," ");
            #allText = allText.replace("<br /><br />","\n");
            #allText = allText.replace("<br/><br/>","\n");
            #allText = allText.replace("<br><br>","\n");
            #allText = allText.replace("<br />","\n");
            #allText = allText.replace("<br/>","\n");
            #allText = allText.replace("<br>","\n");
            #allText = allText.replace("<p>","");
            #allText = allText.replace("</p>","\n");
            #allText = allText.replace("\n\n","\n");
            #allText = allText.replace("\n\n","\n");
            #allText = allText.replace("\n\n","\n");
            #allText = allText.replace("</div>","\n");
            #allText = allText.replace("&ldquo;","\"");
            #allText = allText.replace("&lsquo;","'");
            #allText = allText.replace("&rsquo;","'");
            #allText = allText.replace("&rdquo;","\"");
            #allText = allText.replace("&hellip;","…");
            #allText = allText.replace("&mdash;","—");
            #allText = allText.replace("&amp;","&");
            #allText = allText.replace("&lt;","<");
            #allText = allText.replace("&gt;",">");
            #allText = allText.replace("澹","淡");

            # 使用循环进行替换
            for old, new in replacements.items():
                allText = allText.replace(old, new)
            if removeHTML:
                re.sub(r'([HhＨｈΗ]|[WwＷω]|[MmＭｍＭ])[^\n]{9,100}[MmＭｍＭ]',"",allText); #将各种网址删除的正则(测试)

        else:
            if len(allText)==0:
                #休眠一次时间后重试
                errorTimes +=1;
                if errorTimes==1:
                    print("当前章节指针:"+str(i),"章节名称:",y,"\n","章节网址:",url);
                consoleWrite("[error] ","red");
                print("爬取失败,等待重试中,重试次数:"+str(errorTimes));
                if errorTimes>maxErrorTimes:
                    raise IndexError("爬取第"+str(i+1)+"章节失败.\n章节名称:"+y+"\n章节网址:\n"+url+"\n");
                time.sleep(r.randint(timeWait[0],timeWait[1]));
                continue;
            else:
                for j in range(0,len(allText)):
                    for old, new in replacements.items():
                        allText[j] = allText[j].replace(old,new);
                errorTimes = 0;        

        openWriteAdd("\n\n");
        if haveTitle:
            openWriteAdd(y);
        else:
            openWriteAdd("第"+str(i+1)+"章 "+ y);
        openWriteAdd("\n\n");

        if isLines == False:
            openWriteAdd(allText);                      #单行内容
        else:
            openWrites(allText);                        #多行内容
            #openWrites(allText[:len(allText)-3]);       #去掉最后行尾网站信息
        
        if haveTitle:
            #print("\r",y+"已经下载完成 进度: "+str(math.floor(i/pageCount*10000)/100)+"% ,ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2),end="             ",flush=True);
            consoleWrite(f"[{math.floor(i/pageCount*10000)/100:.2f}%]","green");
            print(format_string3(y)+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2));
        else:
            #print("\r","第"+str(i+1)+"章"+y+"已经下载完成 进度: "+str(math.floor(i/pageCount*10000)/100)+"% ,ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2),end="             ",flush=True);
            consoleWrite(f"[{math.floor(i/pageCount*10000)/100:.2f}%]","green");
            print(format_string3("第"+str(i+1)+"章"+y)+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2));

        i+=1;
        changeIniIndex(i);
        #time.sleep(r.randint(3,7));             #有爬取限制的网站
        #time.sleep(r.randint(0,1));             #无爬取限制的网站
        time.sleep(r.randint(timeWait[0],timeWait[1]));
    print("");
    consoleWrite("小说已经下载完成","DarkGreen");
    print("");

except Exception as e:
    #changeIniIndex(i);
    #print("\x1b[31;1m[Error]\x1b[0m\t" + str(e) + "\n");
    consoleWrite("[Error]","red");
    print(str(e));
    raise e;