import urllib3
import re
import os
import time
import random
import math

from selenium import webdriver;
from selenium.webdriver.chrome.webdriver import WebDriver
from selenium.webdriver.common.keys import Keys;
from selenium.webdriver.common.by import By;
from selenium.webdriver.common.proxy import Proxy,ProxyType;
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities
from selenium.webdriver.remote.webelement import WebElement
from selenium.webdriver.support.ui import WebDriverWait;
from selenium.webdriver.support import expected_conditions as EC;

import undetected_chromedriver as uc;

webUrl = "https://www.nfxs.com/book/120882/";
webUrlForEach = "";
file = "output.txt";
ini = "output.ini";
start = 10 + 1                              #初始推荐章节数量
passUrl = ''                                #排除的对象(URL排除)
passName = "无标题章节";                    #排除的对象(章节名排除)
needProxy = False;                          #下载网站是否需要代理
needVerify = False;                         #是否需要网页ssl证书验证
ignoreDecode = False;                        #忽略解码错误内容
isLines = False;                             #内容是否是多行的
linesRemove = [0,0];                        #多行内容删除(前后各删除几行?)
haveTitle = True;                          #是否有数字章节头(为了小说阅读器辨别章节用)
timeWait = [5,7];                           #等待时间([最小值,最大值])
maxErrorTimes = 1;                          #章节爬取最大错误次数
removeHTML = False;                         #是否移除文章中的URL地址(测试功能)
nextPage = True;                            #是否有更多页(内容是否有第多页)
nextPageStart = 0;                          #分页起始(0或者1)(判断第二页是XX_1.html还是XX_2.html)
maxPages = 2;                               #分页最大限制(-1或者2,3...)(特殊网站XX_6.html还是显示第二页内容,无法触发换页动作)
titleLimit = -1;                            #章节页面显示限制(网页无法显示全部章节,每页只显示多少章节,-1表示全章节显示)
pageStart = 1;                              #章节分页起始页(0或者1)(网页无法显示章节,通常原URL只显示第一部分,这个值表示第二部分是从/1/还是/2/)
pageEndValue = ".html";                     #章节页面页面最后追加内容("/"或者".html"),取决于网站规则
nextPageMerge = "_";                        #章节分页第二页起,新的URL生成规则("_":xx_1.html,":-2":xx01.html)
pageRemove = 10 + 15;                        #章节分页第二页起,推荐章节(或者无用章节)的数量                            
proxyUrl = "http://127.0.0.1:33210";        #代理所使用的地址
usingTools = "urllib3";                     #使用工具[urllib3,selenium或uc](undetected-chromedriver 是一个专为绕过反自动化检测而设计的 ChromeDriver 封装库。它通过隐藏 Selenium 的特征，降低被检测为机器人的可能性。)
pageLoadTimeout = 30                        #页面最大等待时间(单位:秒)(selenium/uc专用)
cssQuery = "#content";                      #css查询节点规则(selenium/uc专用)

#----------------------------------------------------------#
def getForEachUrl(url:str):
    '''
    获取前部拼接头
    '''
    ret = url.split("/");
    r = ret[:3]
    return "/".join(r);
webUrlForEach = webUrlForEach and webUrlForEach or getForEachUrl(webUrl);
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36' }
headers = {
    'User-Agent':'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36',
    # 'Cookie':'Hm_lvt_4a91b5b58ea34940313795aaa14f9fd9=1714749322; Hm_lpvt_4a91b5b58ea34940313795aaa14f9fd9=1714750022'
}
#readDD = re.compile(r'<dd>[\t\0\ \n]*<a href="(.*)"');
#readDD = re.compile(r'<[dd|li]{2} class="col-4">[\t\0\ \n]*<a href="([^"<>]*)"[^<>]*>([^<>]*)<\/a>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt=[^<>]*)? href=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)(<!>)?<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2} class="[^"]+">[\t\0\ \n]*<[Aa] ?(style=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"] ?title=["\']([^"\'<>]*)[\'"][^<>]*>');
# readDD = re.compile(r'<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*><dd>([^<>]*)<\/dd>');
#readDD = re.compile(r'<[dd|li]{2} class="book-item">[\t\0\ \n]*<[Aa] ?(style=?[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[dd|li]{2}>[\t\0\ \n]*<[Aa] ?(style|alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
#readDD = re.compile(r'<[Aa] ?(alt|title=[^<>]*)? href ?=["\']([^"\'<>]*)[\'"][^<>]*>([^<>]*)<\/[Aa]>');
r = random.Random();
iniCount = 4;                               #ini行数
errorTimes = 0;                             #错误次数
if needProxy:                               #设置代理(小飞机)
    os.environ["http_proxy"] = proxyUrl;
    os.environ["https_proxy"] = proxyUrl;

if needVerify==False:
    urllib3.disable_warnings();

#os.environ["TERM"] = "ansi";                #设置环境变量用于显示有色终端内容(无效)
file_path = os.path.abspath(__file__)
dir_path = os.path.dirname(file_path)       #当前文件所在文件夹
reptileGet = True;                          #爬取成功
# 原始代码中的替换操作可以通过创建一个替换字典来简化
replacements = {
    "&nbsp;": " ",
    "&quot;": '"',
    "<br /><br />": "\n",
    "<br/><br/>": "\n",
    "<br><br>": "\n",
    "<br />": "\n",
    "<br/>": "\n",
    "<br>": "\n",
    "<p>": "",
    "</p>": "\n",
    "\t":"",
    "\u3000":" ",
    "    ":" ",
    #"\n\n": "\n",  # 可能需要额外的逻辑来处理连续的换行
    "\r\n":"\n",
    "\n \n": "\n",
    "\n    \n": "\n",
    "\n\n\n":"\n",
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
tempIndex = 0;                              #为了翻页读取内容
noNextPage = False;                         #为了翻页读取内容
startTime = time.time();                    #为了计算爬取时间
isWindows = os.name == 'nt'                 #判断是否是windows系统
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
    # return os._exists(path);
    exists = os.path.exists(path)
    # print(f"file exists: {exists}")
    return exists;
    

def openReadLines(path:str):
    '''
    打开文件读取里面的所有行
    '''
    ret = []
    if exists(path)==False:
        raise Exception(path+"不存在!");
    with open(path,"r",encoding="utf-8") as f:
        allValue = f.readlines();
        ret = allValue;
    return ret;

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
    second = math.floor(second);
    s = second % 60
    second = second // 60
    m = second % 60
    second = second // 60
    h = second % 24
    second = second // 24
    ret = second and str(second) + "天" or ""
    ret += h and str(h) + "小时" or ""
    ret += m and str(m) + "分钟" or ""
    ret += str(s) + "秒"
    return ret;

def getTime2(second:int):
    '''
    根据秒计算时间
    英文显示
    '''
    second = math.floor(second);
    s = second % 60
    second = second // 60
    m = second % 60
    second = second // 60
    h = second % 24
    second = second // 24
    ret = second and str(second) + "d " or ""
    ret += str(h) + ":"
    ret += str(m) + ":"
    ret += str(s)
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
    
    根据系统判断调用
    '''
    try:
        if isWindows:
            # 使用windows系统情况下,调用c#程序打印有颜色的字符串
            value = dir_path+"\\write.exe "+text+" -C "+color;
            os.system(value);
        else:
            write(text,color);
    except Exception:
        print(text,end="");

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

def getAllDD(http,i:int):
    '''
    获取章节
    '''
    # 处理页面url
    # get请求指定网址
    if titleLimit>=0:
        newUrl = webUrl + str(i+pageStart) + pageEndValue;
        #res = http.request("GET",webUrl)
        res = http.request("GET",newUrl,None,headers);

    else:
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

    # # 获取HTTP状态码
    # print("status:%d" % res.status)

    try:
        # 获取响应内容
        data = res.data.decode("utf-8")
            
    except UnicodeDecodeError as err:
        data = res.data.decode("gbk");

    #data.replace("\<!\>","")              #删除特殊注释
    allDD = readDD.findall(data);
    allDD2 = [];
    for x in allDD:
        t = [];
        for y in x:
            t.append(y.strip());
        allDD2.append(t);
    allDD = allDD2;
    
    #消除初始章节
    if i==0:
        #消除初始推荐(第一页推荐章节)章节
        allDD = allDD[start:]
    else:
        #清除非第一页推荐章节
        allDD = allDD[pageRemove:];

    if titleLimit > 0:
        allDD = allDD[:titleLimit];
    
    if len(allDD)>0:
        print("["+allDD[0][-1]+"..."+allDD[-1][-1]+"]");
    else:
        print("[]");
    return allDD;

class SeleniumHttpResponse:
    '''
    用于转换selenium返回结果
    '''
    def __init__(self,url,data:str,status,driver:WebDriver):
        self.url = url;
        self.data = data.encode("utf-8");
        self.status = status;
        self.driver = driver;

    def geturl(self):
        '''
        获取URL
        '''
        return self.url;

    def getDom(self,rule):
        '''
        获取节点对象(使用css规则)
        '''
        try:
            return self.driver.find_elements(By.CSS_SELECTOR,rule);
        except Exception:
            return None;

class Http:
    '''
    用于转换selenium
    '''
    
    def __init__(self,driver:WebDriver):
        self.driver = driver;
        self._request_count = 0;    #调用了几次request功能(用于在第一次调用时过反爬虫检测)
    
    def request(self,mothed="GET",url="",body=None,fields=None,headers={},json={}):
        global errorTimes;

        driver = self.driver;
        #访问网站
        try:
            driver.get(url);
        except Exception as e:
            print("页面加载超时，停止加载...")
            driver.execute_script("window.stop();")  # 停止页面加载
            if input("回车继续执行代码,输入内容后回车则退出并显示报错") == "":
                pass
            else:
                raise e;

        #第一次访问打印访问内容标题
        if self._request_count==0:
            print(driver.title);
        self.__close_else__();

        #等待
        wait = WebDriverWait(driver,10);
        wait.until(EC.visibility_of_element_located((By.TAG_NAME, "body")))

        #读取内容判断是否触发反爬虫机制
        data = driver.page_source;
        while 'Cloudflare' in data: #检测是否有Cloudflare保护
            input("等待反爬机制审查,通过请按下回车键继续执行下一步...");
            #需要重新读取页面内容
            data = driver.page_source;
            if errorTimes < maxErrorTimes:
                errorTimes += 1;
            else:
                raise("连续的发爬虫审查!请检察是否有误");
    

        #显示全部章节的结构需要翻页到最底部
        if titleLimit == -1 and self._request_count == 0:
            js = driver.execute_script      #获取JavaScriptExecutor
            #执行向下滚轮脚本
            js('''
                var callback = arguments[arguments.length - 1];
                window.scrollTo = function (y, duration) {
                    duration = duration * 1000;
                    var scrollTop = document.documentElement.scrollTop || document.body.scrollTop || 0;
                    var distance = y - scrollTop;
                    var scrollCount = duration / 10;
                    var everyDistance = distance / scrollCount;
                    var count = 0;
                    for (var index = 1; index <= scrollCount; index++) {
                        setTimeout(function () {
                            window.scrollBy(0, everyDistance);
                            count++;
                            if (count === scrollCount) {
                                callback(); // 在滚动完成后调用回调
                            }
                        }, 10 * index);
                    }
                };
                scrollTo(1000000, 60); // 用一分钟翻页，降低爬虫被 ban 的可能性
            ''');

        #返回转换后的结果
        res = SeleniumHttpResponse(
            url=driver.current_url,
            data=data,
            status=200,
            driver=driver
        );
        
        #调用接口计数加1
        self._request_count += 1;
        
        return res;

    def __close_else__(self):
        '''
        关闭除了当前窗口外的所有窗口
        '''
        driver = self.driver;
        # 获取当前所有窗口句柄
        window_handles = driver.window_handles
        main_window = driver.current_window_handle  # 主窗口句柄

        # 遍历所有窗口句柄，关闭非主窗口
        for handle in window_handles:
            if handle != main_window:
                driver.switch_to.window(handle)
                driver.close()

        # 回到主窗口
        driver.switch_to.window(main_window)

try:
    # 实例化产生请求对象
    if usingTools == "urllib3":
        if needProxy:
            http = urllib3.ProxyManager(proxyUrl,headers = headers,cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"));
        else:
            http = urllib3.PoolManager(cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"))
    elif usingTools == "selenium" or usingTools == "uc":        
        #设置启动参数
        options = webdriver.ChromeOptions();
        options.add_argument("--remote-allow-origins=*")    # 允许所有请求
        options.add_argument("--disable-gpu")               # 禁用 GPU
        options.add_argument("--no-sandbox")                # 禁用沙盒
        options.add_argument("lang=zh_CN.UTF-8")            # 设置语言
        caps = DesiredCapabilities.CHROME                   # 配置日志
        caps["goog:loggingPrefs"] = {"browser": "ALL"}

        # 设置页面加载策略
        options.page_load_strategy = "normal"

        #判断是否需要代理
        if needProxy:
            proxy = Proxy();
            proxy.proxyType = ProxyType.MANUAL;
            proxy.http_proxy = proxyUrl;
            proxy.ssl_proxy = proxyUrl;
            
            #添加代理
            options.proxy = proxy;

        #创建浏览器实例
        # driver = webdriver.Chrome(options=options,desired_capabilities=caps);
        if usingTools == "uc":
            driver = uc.Chrome(options=options);
        else:
            driver = webdriver.Chrome(options=options);
        
        #设置最大等待时间
        driver.set_page_load_timeout = pageLoadTimeout;
        
        #翻译结构
        http = Http(driver);

    allDD = [];
    urladds = [];
    names = [];
    i = 0;

    if titleLimit>= 0:
        tempDD = getAllDD(http,i);
        while len(tempDD)==titleLimit:
            for x in range(0,titleLimit):
                allDD.append(tempDD[x]);
            i+=1;
            # 等待,防止被ban
            time.sleep(r.randint(timeWait[0],timeWait[1]));
            tempDD = getAllDD(http,i);
        # 加入最后一部分章节
        for x in range(0,len(tempDD)):
            allDD.append(tempDD[x]);
    else:
        allDD = getAllDD(http,i);
    
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

    need = len(urladds);
    pageCount = len(allDD) or 1;         #总章节数量
    startTime = time.time();        #用来计算用时

    while(i<len(names)):        
        x = urladds[i];
        y = names[i];
        url = webUrlForEach+x;
        if url == webUrlForEach+passUrl or y==passName:
            i+=1;
            continue;
        
        def getPage(url):
            global tempIndex;
            global errorTimes;
            global noNextPage;
            
            #print(url);
            res = http.request("GET",url,None,headers);
            #print(res.status);
            try:
                eachData = res.data.decode("utf-8");
            except UnicodeDecodeError as err2:
                eachData = res.data.decode("gbk",errors= (ignoreDecode==False and 'replace'or'ignore'));
            #print(eachData);
            
            #判断页面是否被重定向(如果被重定向就判断,此章节没有更多页面内容了)
            nowUrl = res.geturl();
            nowUrl = (nowUrl.startswith("http")==False and webUrlForEach or "") + (nowUrl and nowUrl.split("?")[0] or "");
            
            if nowUrl != url:
                noNextPage = True;
                return;  
        
            #通过最大页面数量限制(这种方式比较蠢,但是有些网站XX_6.html依旧显示第二页内容,且URL地址不变更,没有别的办法)
            if maxPages>0:
                if (tempIndex + nextPageStart) > maxPages:
                    noNextPage = True;
                    return;
                    
            if isLines==False:
                #text = re.compile(r'<div id="chaptercontent"[^<>]*>([\s\S]*)'+webUrlForEach)
                #text = re.compile(r'div id="content">([\s\S]*)'+webUrlForEach.split("/")[-1])
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
                text = re.compile(r'div id="content">([\s\S]*)<script>read3')
                #text = re.compile(r'div id="content">([\s\S]*)<div class="bottem2">')
                #text = re.compile(r'div id="content">([\s\S]*)<\/div>[\n\t\0\r\ ]*<script>read3')
                #text = re.compile(r'div id="content">([\s\S]*)<br /><br />\(https')
                #text = re.compile(r'div id="content" deep="3">([\s\S]*)<br><br>\n为您提供大神薪意')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)[\r\n]*<a href="javascript:;" on')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)[\r\n]*<a href="javascript:posterror')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)[\r\n]*<br>网页版章节内容慢')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)无尽的昏迷过后')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)有的人死了，但没有完全死……')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)<script>read3')
                #text = re.compile(r'<div id="content">([\s\S]*)<div id="center_tip">')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)<div id="center_tip">')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)<div align="center">')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)浩瀚的宇宙中，一片星系的生灭')
                #text = re.compile(r'id=["\']content["\']>([\s\S]*)id=["\']contentdec["\']><div')
                #text = re.compile(r'<div id="content">([\s\S]*)[\r\n]*<br>网页版章节内容慢')
                #text = re.compile(r'<div id="content" deep="3">([\s\S]*)无尽的昏迷过后')
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
                #text = re.compile(r'<script>read2\(\);</script>([\s\S]*)<script>read3')
                #text = re.compile(r'<script>app2\(\);</script>([\s\S]*)<script>app2\(\);</script>')
                #text = re.compile(r'<div id="chaptercontent" class="Readarea ReadAjax_content">([\s\S]*)<p class="readinline">')
                #text = re.compile(r'<div id="chaptercontent" class="Readarea ReadAjax_content">([\s\S]*)请收藏本站：http')
                #text = re.compile(r'<div id="htmlContent">([\s\S]*)<div class="bottem">')
                #text = re.compile(r'<div id="conter_tip">([\s\S]*)<div id="conter_tip">')
            else:
                #text = re.compile(r'<p class=".*">([^<>]*)<\/p>')
                text = re.compile(r'<p>([^<>]*)<\/p>')
        
                #eachData = eachData.replace("\x3C","<");    #修复特殊字符

            if usingTools == "urllib3":
                allText = text.findall(eachData);
            else:
                doms = res.getDom(cssQuery);
                allText = [];
                for x in doms:
                    allText.append(x.text);                
            
            if nextPage and len(allText)==0:
                noNextPage = True;
                return;

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
                    noNextPage = True;
                    return;

                # 使用循环进行替换
                for old, new in replacements.items():
                    allText = allText.replace(old, new)
                    # 处理字符串前后的空白字符串
                    allText = allText.strip();
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
                    noNextPage = True;
                    return;
                else:
                    for j in range(linesRemove[0],len(allText) - linesRemove[1]):
                        for old, new in replacements.items():
                            allText[j] = allText[j].replace(old,new);
                        #去掉行前后的空白字符串
                        allText[j] = allText[j].strip();
                    errorTimes = 0;        

            openWriteAdd("\n\n");
            if tempIndex==0:
                if haveTitle:
                    openWriteAdd(y);
                else:
                    openWriteAdd("第"+str(i+1)+"章 "+ y);
                openWriteAdd("\n\n");
            elif nextPage:
                openWriteAdd("(第"+str(tempIndex+(2-nextPageStart))+"页)")
                openWriteAdd("\n\n");
            tempIndex+=1;                                   #页面增加

            if isLines == False:
                openWriteAdd(allText);                      #单行内容
            else:
                rets = [];
                for x in range(linesRemove[0],len(allText) - linesRemove[1]): #忽略行
                    rets.append(allText[x]);
                openWrites(rets);                        #多行内容
                #openWrites(allText[:len(allText)-3]);       #去掉最后行尾网站信息
            
            #用于计算当前用时
            nowTime = time.time();
            if haveTitle:
                consoleWrite(f"[{math.floor(i/pageCount*10000)/100:.2f}%]","green");
                
                if nextPage==False:
                    print(format_string3(y)+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2)+" UT: "+getTime2(nowTime - startTime));
                else:
                    print(format_string3(y+" "+str(tempIndex)+"页")+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2)+" UT: "+getTime2(nowTime - startTime));
            else:
                consoleWrite(f"[{math.floor(i/pageCount*10000)/100:.2f}%]","green");
                if nextPage==False:
                    print(format_string3(y)+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2)+" UT: "+getTime2(nowTime - startTime));
                else:
                    print(format_string3(y+" "+str(tempIndex)+"页")+"已经下载完成    ETA: "+getTime((pageCount-i)*(timeWait[0]+timeWait[1])//2)+" UT: "+getTime2(nowTime - startTime));

        getPage(url);
        while nextPage and noNextPage==False:
            tempUrls = url.split("/");
            tempI = len(tempUrls) - 1;
            for ti in range(len(tempUrls)-1,-1,-1):
                if tempUrls[ti] != "":
                    tempParts = tempUrls[ti].split(".");
                    readPart = re.compile("\d$");
                    for tj in range(len(tempParts)-1,-1,-1):
                        if len(readPart.findall(tempParts[tj]))==1:
                            if nextPageMerge == "_":
                                tempParts[tj] = tempParts[tj]+"_"+str(tempIndex+nextPageStart);
                            elif nextPageMerge == ":-2":
                                tempParts[tj] = tempParts[tj][:-2]+"{0:02d}".format(tempIndex+nextPageStart);
                            tempUrls[ti] = ".".join(tempParts);
                            break;
                    break;
            tempUrl = "/".join(tempUrls);
            getPage(tempUrl);

        i+=1;
        tempIndex = 0;
        noNextPage = False;
        changeIniIndex(i);
        time.sleep(r.randint(timeWait[0],timeWait[1])); #等待时间绕过爬取限制
    print("");
    consoleWrite("小说已经下载完成","DarkGreen");
    endTime = time.time();
    consoleWrite("总计用时: "+getTime(math.floor(endTime - startTime)),"DarkCyan");
    print("");

except Exception as e:
    #changeIniIndex(i);
    consoleWrite("[Error]","red");
    print(str(e));
    #因为无法调试,现在需要抛出当前状态
    consoleWrite("[Url]","green");
    try:
        consoleWrite(str(url),"green");
    except:
        consoleWrite("URL不存在!","red");
    print("");
    # consoleWrite("[Value]","white");
    # consoleWrite(str(eachData),"white");
    raise e;