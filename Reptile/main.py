import urllib3
import math
import argparse
import sys

# 设置命令行参数解析
parser = argparse.ArgumentParser(description='网页内容爬取脚本。')
parser.add_argument('-U','--url',type=str,default="www.baidu.com",help='访问url')
parser.add_argument('-O','--out',type=str,help='输出到指定txt文件中')
parser.add_argument('-P','--proxy',type=str,default="http://127.0.0.1:33210",help="设置代理路径")
parser.add_argument("-M",'--max',type=int,default=5,help="最大重试次数")
parser.add_argument("-E","--endText",type=str,default="___END___",help="为了防止流读取出错设置的结尾字符串")

parser.add_argument('-NP','--needProxy',action='store_true',help="是否需要代理")
parser.add_argument('-NV','--needVerify',action='store_true',help="是否需要网页SSL证书验证")

#--------------------------------------------------------#
args = parser.parse_args()
#设置参数
needProxy = args.needProxy
needVerify = args.needVerify
webUrl = args.url
output = args.out
endText = args.endText
maxErrorTimes = args.max;
headers = {'User-Agent':'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36' }
errorTimes = 0;                 #错误次数
if args.needProxy:              #设置代理
    os.environ["http_proxy"] = args.proxy
    os.environ["https_proxy"] = args.proxy

if args.needVerify==False:      #禁用SSL验证
    urllib3.disable_warnings(); 
#--------------------------------------------------------#

if __name__ == "__main__":
    while errorTimes<maxErrorTimes:
        try:
            if needProxy:
                http = urllib3.ProxyManager("http://127.0.0.1:33210",headers = headers,cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"));
            else:
                http = urllib3.PoolManager(cert_reqs = (needVerify==False and 'CERT_NONE' or "CERT_REQUIRED"))

            # get请求指定网址
            res = http.request("GET",webUrl,None,headers);

            try:
                # 获取响应内容
                data = res.data.decode("utf-8")
            
            except UnicodeDecodeError as err:
                data = res.data.decode("gbk");

            try:
                if output:
                    with open(output,"w",encoding="utf8") as f:
                        f.write(data);
            except Exception as ioE:
                pass

            print(data);
            print(endText);
            
            #成功运行,退出程序
            sys.exit();
        except Exception as error:
            errorTimes += 1;
            if errorTimes==maxErrorTimes:
                print(error);
                print(endText);

