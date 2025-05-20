import subprocess
import argparse

def main():
    # 创建参数解析器
    parser = argparse.ArgumentParser(description="运行 GetCK.py 和 getSCHL.py")
    parser.add_argument("--username", required=True, help="用户名")
    parser.add_argument("--password", required=True, help="密码")
    
    # 解析参数
    args = parser.parse_args()
    username = args.username
    password = args.password

    # 运行 GetCK.py 抓取 cookies
    subprocess.run(["python", "GetCK_AUTO.py", username, password], check=True)

    # 运行 getSCHL.py 抓取数据
    subprocess.run(["python", "getSCHL.py"], check=True)

if __name__ == "__main__":
    main()
