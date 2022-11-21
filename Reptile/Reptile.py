import urllib3
import re
import os
import time
import random

webUrl = "https://www.lvsewx.com/books/33/33642/53999959.html";
webUrlForEach = "https://www.lvsewx.com";
file = "output.txt";
ini = "ouput.ini";
start = 20               #初始推荐章节数量
passUrl = '/html/13/13722/7099871.shtml'   #排除的对象(URL排除)
passName = "无标题章节";                    #排除的对象(章节名排除)

#----------------------------------------------------------#
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36','Cookie':'fikker-TiPI-ZIdg=rmfYpsRWNibUbSRID3fMT3LwMIvenNb3; fikker-TiPI-ZIdg=rmfYpsRWNibUbSRID3fMT3LwMIvenNb3; bgcolor=; font=; size=; fontcolor=; width=; bookid=36090%2C36090; Hm_lvt_20aa077072a9d85797a5443f74cc080e=1664966679,1664970894; chapterid=58701020%2C58701751; chaptername=%u7B2C1%u7AE0%20%u624B%u4E2D%u63E1%u7684%u4FBF%u662F%u6574%u4E2A%u4EBA%u751F%2C%u7B2C712%u7AE0%20%u7EFF%u9152%u65B0%u8BCD; Hm_lpvt_20aa077072a9d85797a5443f74cc080e=1664970936'  }
#readDD = re.compile(r'<dd>[\t\0\ \n]*<a href="(.*)"');
#readDD = re.compile(r'<[dd|li]{2} class="col-4">[\t\0\ \n]*<a href="([^"<>]*)"[^<>]*>([^<>]*)<\/a>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt=[^<>]*)? href=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)(<!>)?<\/[Aa]>');
readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
r = random.Random();
iniCount = 4;                               #ini行数
#----------------------------------------------------------#

def openWriteAdd(s:str):
    '''
    打开文件写内容
    '''
    with open(file,"a",encoding="utf-8") as f:
        f.write(s);
    return;

def openWtite(path:str,s:str):
    '''
    打开一个不存在的文件并写内容
    '''
    with open(path,"w",encoding="utf-8") as f:
        f.write(s);
    return;

def openWrites(s:list):
    '''
    打开文件写内容
    '''
    with open(file,"a",encoding="utf-8") as f:
        for x in s:
            x = x.replace(" ","");
            x = x.replace("\n","");
            x = x.replace("\r","");
            f.writelines(x+"\n");
    return;

def exists(path:str):
    '''
    判断文件是否存在
    '''
    return os._exists(path);

def openReadLines(path:str):
    '''
    打开文件读取里面的所有行
    '''
    with open(path,"r",encoding="utf-8") as f:
        allValue = f.readlines();
        return allValue;
    return []

def saveIni(url:str,urladds:list,names:list,index:int):
    '''
    保存INI
    '''
    openWtite(ini,"URL:"+str(url)+"\nURLADDS:"+','.join(urladds)+"\nNAMES:"+','.join(names)+"\nINDEX:"+str(index));
    return;

def changeIniIndex(index:int):
    '''
    修改ini中指针指向
    '''
    lines = openReadLines(ini);
    lines[3] = "INDEX:"+str(index);
    openWtite(ini,"".join(lines));
    return;

try:
    # 实例化产生请求对象
    http = urllib3.PoolManager()

    # get请求指定网址
    res = http.request("GET",webUrl)

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

    urladds = urladds[i:]             #跳过已有章节
    names = names[i:]             #跳过已有章节
    need = len(urladds);
    
    for j in range(0,need):
        
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

        
        x = urladds[j];
        y = names[j];
        url = webUrlForEach+x;
        if url == webUrlForEach+passUrl or y==passName:
            i+=1;
            continue;
        res = http.request("GET",url);
        #print(res.status);
        try:
            eachData = res.data.decode("utf-8");
        except UnicodeDecodeError as err2:
            eachData = res.data.decode("gbk");

        #print(eachData);
        
        #text = re.compile(r'<p class=".*">([^<>]*)<\/p>')
        #text = re.compile(r'<p>([^<>]*)<\/p>')
        #text = re.compile(r'div id="content">([\s\S]*)<\/div>\n<a')
        #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\r\n]*<a')
        text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<\/div>\n<script>read3')
        #text = re.compile(r'<script>read2();</script>([\s\S]*)<script>app2();</script>')
        
        allText = text.findall(eachData);

        allText = allText[0];
        allText = allText.replace("&nbsp;"," ");
        allText = allText.replace("<br /><br />","\n");
        allText = allText.replace("<br />","\n");
        allText = allText.replace("\n\n","\n");
        #allText = allText.replace("\n\n","\n");
        #allText = allText.replace("\n\n","\n");
        
        openWriteAdd("\n");
        #openWriteAdd("第"+str(i)+"章 "+ y);
        openWriteAdd(y);
        openWriteAdd("\n\n");

        #openWrites(allText);                       #多行内容
        #openWrites(allText[:len(allText)-3]);       #去掉最后行尾网站信息
        openWriteAdd(allText);                      #单行内容
        
        print("第"+str(i)+"章已经下载完成");
        i+=1;
        changeIniIndex(i);
        time.sleep(r.randint(0,1));

except Exception as e:
    #changeIniIndex(i);
    raise e;