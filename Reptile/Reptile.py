import urllib3
import re
import os

webUrl = "https://www.fpzw.org/xiaoshuo/7/7138/";
headers = {'User-Agent':'Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.6) Gecko/20091201 Firefox/3.5.6'  }
file = "output.txt";
readDD = re.compile(r'<dd><a href="(.*)"');

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
    with open(file,"a","utf-8") as f:
        for x in s:
            f.writelines(s[x]);
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

    i = 0;
    for x in allDD:
        url = webUrl+x;
        res = http.request("GET",url);
        #print(res.status);
        try:
            eachData = res.data.decode("utf-8");
        except UnicodeDecodeError as err2:
            eachData = res.data.decode("gbk");

        #print(eachData);
        text = re.compile(r'</script>(.+)</p></div>')
        allText = text.findall(eachData);
        allText = allText[0];
        allText = allText.replace("&nbsp;"," ");
        allText = allText.replace("<br /><br />","\n");
        openWriteAdd(allText);
        i+=1;
        print("第"+str(i)+"章已经下载完成");

except Exception as e:
    raise e;