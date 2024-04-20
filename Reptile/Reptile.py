import urllib3
import re
import os
import time
import random
import math

webUrl = "https://www.xswang.vip/book/65942/";
webUrlForEach = "https://www.x23wxw6.com";
file = "output.txt";
ini = "output.ini";
start = 10 + 13                              #初始推荐章节数量
passUrl = ''                                #排除的对象(URL排除)
passName = "无标题章节";                    #排除的对象(章节名排除)
needProxy = False;                          #下载网站是否需要代理
needVerify = True;                         #是否需要网页ssl证书验证
ignoreDecode = False;                        #忽略解码错误内容
isLines = False;                             #内容是否是多行的
haveTitle = True;                          #是否有数字章节头(为了小说阅读器辨别章节用)
timeWait = [1,3];                           #等待时间([最小值,最大值])
maxErrorTimes = 10;                          #章节爬取最大错误次数
removeHTML = False;                         #是否移除文章中的URL地址(测试功能)

#----------------------------------------------------------#
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36' }
#readDD = re.compile(r'<dd>[\t\0\ \n]*<a href="(.*)"');
#readDD = re.compile(r'<[dd|li]{2} class="col-4">[\t\0\ \n]*<a href="([^"<>]*)"[^<>]*>([^<>]*)<\/a>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt=[^<>]*)? href=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)(<!>)?<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2} class="book-item">[\t\0\ \n]*<[Aa] ?(style=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2} class="book-item">[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style|alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[Aa] ?(alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
r = random.Random();
iniCount = 4;                               #ini行数
errorTimes = 0;                             #错误次数
if needProxy:                               #设置代理(小飞机)
    os.environ["http_proxy"] = "http://127.0.0.1:33210";
    os.environ["https_proxy"] = "http://127.0.0.1:33210";

if needVerify==False:
    urllib3.disable_warnings();

#os.environ["TERM"] = "ansi";                #设置环境变量用于显示有色终端内容(无效)
file_path = os.path.abspath(__file__)
dir_path = os.path.dirname(file_path)       #当前文件所在文件夹
reptileGet = True;                          #爬取成功
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

def getTime(second:int):
    '''
    根据秒计算时间
    '''
    s = second % 60
    second = second // 60
    m = second % 60
    second = second // 60
    h = second % 24
    second = second // 24
    ret = second and str(second) + "天" or ""
    ret+= h and str(h) + "小时" or ""
    ret += m and str(m) + "分钟" or ""
    ret += str(s) + "秒"
    return ret;

def write(text, color):
    '''
    打印有颜色的字,不换行
    '''
    colors = {
        'red': '\033[91m',
        'green': '\033[92m',
        'yellow': '\033[93m',
        'blue': '\033[94m',
        'purple': '\033[95m',
        'white': '\033[97m',
        'grey': '\033[90m',
        'black': '\033[30m',
        'default': '\033[0m',
    }
    print(colors[color] + str(text) + colors['default'],end="");
    
def writeLine(text, color):
    '''
    打印有颜色的字
    '''
    colors = {
        'red': '\033[91m',
        'green': '\033[92m',
        'yellow': '\033[93m',
        'blue': '\033[94m',
        'purple': '\033[95m',
        'white': '\033[97m',
        'grey': '\033[90m',
        'black': '\033[30m',
        'default': '\033[0m',
    }
    print(colors[color] + str(text) + colors['default']);

def consoleWrite(text,color):
    '''
    调用c#写控制台软件
    用于写有颜色的字符串
    '''
    value = dir_path+"\\write.exe "+text+" -C "+color;
    os.system(value);

def format_string(s,max_len=20):
    '''
    限制字符串长度,
    如果超过限制则只打印max_len-3的内容和三个点
    如果不足则在后面补足空格
    '''
    if len(s) > max_len:
        s = s[:17] + "..."
    else:
        s = s.ljust(max_len)
    s = to_fullwidth(s);
    return s;

def format_string2(s,max_len=20):
    '''
    限制字符串长度,
    如果超过限制则只打印max_len-3的内容和三个点
    如果不足则在后面补足空格
    字符串中每有一个半角字符,就补一个空格
    '''
    for x in s:
        code = ord(x);
        if 32<=code<=126:
            max_len = max_len + 1;

    if len(s) > max_len:
        s = s[:max_len-3] + "..."
    else:
        s = s.ljust(max_len)
            
    return s;

def format_string3(s, max_len=20):
    '''
    限制字符串长度,
    如果超过限制则只打印max_len-3的内容和三个点
    如果不足则在后面补足空格
    字符串中每有一个半角字符,就补一个空格
    '''
    # 初始化实际占用宽度
    display_width = 0
    
    # 对字符串进行遍历，累计每个字符的显示宽度
    for x in s:
        code = ord(x)
        if 32 <= code <= 126:  # 对于半角字符，宽度增加2
            display_width += 2
        else:  # 对于非半角字符，宽度增加1
            display_width += 1
            
    # 如果计算的显示宽度超过最大宽度，需要截断
    if display_width > max_len:
        adjusted_width = 0
        for i, char in enumerate(s):
            code = ord(char)
            if 32 <= code <= 126:
                adjusted_width += 2
            else:
                adjusted_width += 1
            # 确定截断位置
            if adjusted_width + 3 > max_len:
                s = s[:i] + "..."
                break
    # 如果不足最大宽度，填充空格
    else:
        extra_spaces = (max_len - display_width) // 2
        s = s + ' ' * extra_spaces
            
    return s;

def format_string3(s, max_len=30):
    '''
    限制字符串长度,
    如果超过限制则只打印max_len-3的内容和三个点
    如果不足则在后面补足空格
    字符串中每有一个半角字符,就补一个空格
    '''
    # 初始化实际占用宽度
    display_width = 0
    # 确定插入省略号的位置
    cut_off_point = max_len - 3

    # 对字符串进行遍历，累计每个字符的显示宽度
    for i, char in enumerate(s):
        code = ord(char)
        if 32 <= code <= 126:  # 半角字符
            display_width += 1
        else:  # 全角字符
            display_width += 2

        # 检查是否需要截断并添加省略号
        if display_width >= cut_off_point:
            # 截取字符串直到截断点，加上省略号
            s = s[:i] + "..."
            break
    else:  # 如果循环结束后，宽度还不足max_len
        # 添加空格到字符串末尾以达到max_len
        s += ' ' * (max_len - display_width)

    return s;

def format_string3(s, max_len=30):
    '''
    格式化字符串到给定的显示宽度。
    全角字符算作2个宽度单位，半角字符算作1个宽度单位。
    如果字符串宽度超出max_len，则截断并在末尾添加省略号。
    如果字符串宽度不足max_len，则在末尾添加空格。
    '''
    max_len *= 2  # 由于全角字符计为2，所以乘以2
    cur_len = 0
    for i, char in enumerate(s):
        if 32<= ord(char) <= 126:
            cur_len += 1
        else:
            cur_len += 2

        if cur_len > max_len:
            s = s[:i] + "..."
            break
    else:
        s += ' ' * (max_len - cur_len)  # 添加空格补足长度

    return s

def to_fullwidth(s):
    '''
    将字符串中的内容全部转为全角字符串
    '''
    fullwidth_chars = ""
    for char in s:
        code = ord(char)
        # ASCII字符的码点从33到126可以直接转换为全角字符
        if 33 <= code <= 126:
            fullwidth_chars += chr(code + 65248)
        # 空格字符特殊处理
        elif code == 32:
            fullwidth_chars += chr(12288)
        else:
            fullwidth_chars += char
    return fullwidth_chars

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
        
        
        if isLines==False:
            #text = re.compile(r'<div id="chaptercontent"[^<>]*>([\s\S]*)'+webUrlForEach)
            #text = re.compile(r'div id="content">([\s\S]*)<\/div>\n<a')
            #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\r\n]*<a')
            #text = re.compile(r'<div class="posterror">([\s\S]*)[\r\n]*<a href="javascript:;" on')
            #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\r\n\t\ ]*<div class="bottem2">')
            #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\r\n\t\ ]*<div class="readerFooterNav"')
            #text = re.compile(r'div id="content">([\s\S]*)<br /><br /><p>')
            #text = re.compile(r'div id="content">([\s\S]*)<p>三月，初春。<\/p>')
            #text = re.compile(r'div id="content">([\s\S]*)<br /><br />.https:')
            #text = re.compile(r'<div class="content" id="chaptercontent">([\s\S]*)<div class="info bottominfo">')
            #text = re.compile(r'<div id="content" name="content">([\s\S]*)<center class="clear">')
            #text = re.compile(r'<div class="content" id="content">([\s\S]*)<div class="section-opt')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<\/div>\n<script>read3')
            #text = re.compile(r'div id="content">([\s\S]*)<script>read3')
            #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\n\t\0\r\ ]*<script>read3')
            #text = re.compile(r'div id="content">([\s\S]*)<br /><br />\(https')
            #text = re.compile(r'div id="content" deep="3">([\s\S]*)<br><br>\n为您提供大神薪意')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)[\r\n]*<a href="javascript:;" on')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)[\r\n]*<br>网页版章节内容慢')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)无尽的昏迷过后')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)有的人死了，但没有完全死……')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)<script>read3')
            #text = re.compile(r'<div id="content" deep="3">([\s\S]*)<div id="center_tip">')
            #text = re.compile(r'<div id="content">([\s\S]*)[\r\n]*<br>网页版章节内容慢')
            text = re.compile(r'<div id="content" deep="3">([\s\S]*)无尽的昏迷过后')
            #text = re.compile(r'div id="content">([\s\S]*)无尽的昏迷过后')
            #text = re.compile(r'div id="content">([\s\S]*)有的人死了，但没有完全死……')
            #text = re.compile(r'div id="content" deep="3">([\s\S]*)有的人死了，但没有完全死……')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<script')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<script>read3')
            #text = re.compile(r'div id="content">([\s\S]*)<script>read3')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<script>showByJs')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<div class="page_chapter">')
            #text = re.compile(r'div id="content" class="showtxt">([\s\S]*)<script>app2\(\);</script>')
            #text = re.compile(r'<script>read2\(\);</script>([\s\S]*)<script>app2\(\);</script>')
            #text = re.compile(r'<script>app2\(\);</script>([\s\S]*)<script>app2\(\);</script>')
            #text = re.compile(r'<div id="chaptercontent" class="Readarea ReadAjax_content">([\s\S]*)<p class="readinline">')
            #text = re.compile(r'<div id="htmlContent">([\s\S]*)<div class="bottem">')
        else:
            #text = re.compile(r'<p class=".*">([^<>]*)<\/p>')
            text = re.compile(r'<p>([^<>]*)<\/p>')
        
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