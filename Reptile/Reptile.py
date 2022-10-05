import urllib3
import re
import os
import time

webUrl = "http://www.26ksw.cc/book/36090/";
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36','Cookie':'fikker-TiPI-ZIdg=rmfYpsRWNibUbSRID3fMT3LwMIvenNb3; fikker-TiPI-ZIdg=rmfYpsRWNibUbSRID3fMT3LwMIvenNb3; bgcolor=; font=; size=; fontcolor=; width=; bookid=36090%2C36090; Hm_lvt_20aa077072a9d85797a5443f74cc080e=1664966679,1664970894; chapterid=58701020%2C58701751; chaptername=%u7B2C1%u7AE0%20%u624B%u4E2D%u63E1%u7684%u4FBF%u662F%u6574%u4E2A%u4EBA%u751F%2C%u7B2C712%u7AE0%20%u7EFF%u9152%u65B0%u8BCD; Hm_lpvt_20aa077072a9d85797a5443f74cc080e=1664970936'  }
file = "output.txt";
readDD = re.compile(r'<dd>[\t\0\ \n]*<a href="(.*)"');

def openWriteAdd(s:str):
    '''
    打开文件写内容
    '''
    with open(file,"a",encoding="utf-8") as f:
        f.write(s);
    return;

def openWtite(s:str):
    '''
    打开一个不存在的文件并写内容
    '''
    return

def openWrites(s:list):
    '''
    打开文件写内容
    '''
    with open(file,"a",encoding="utf-8") as f:
        for x in s:
            x = x.replace(" ","");
            x = x.replace("\n\n","\n");
            f.writelines(x);
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

    allDD = readDD.findall(data);
    allDD = allDD[12:]              #消除初始推荐章节

    i = 0;
    for x in allDD:
        y = x.split("/")
        url = webUrl+y[len(y)-1];
        res = http.request("GET",url);
        #print(res.status);
        try:
            eachData = res.data.decode("utf-8");
        except UnicodeDecodeError as err2:
            eachData = res.data.decode("gbk");

        #print(eachData);
        text = re.compile(r'<p class=".*">([^<>]*)<\/p>')
        allText = text.findall(eachData);
        #allText = allText[0];
        #allText = allText.replace("&nbsp;"," ");
        #allText = allText.replace("<br /><br />","\n");
        #openWriteAdd(allText);
        openWrites(allText);
        i+=1;
        print("第"+str(i)+"章已经下载完成");
        time.sleep(10);

except Exception as e:
    raise e;