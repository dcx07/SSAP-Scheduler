# -*- coding: utf-8 -*-
import subprocess
import json
import os

    

def main():
   
    # 运行 GetCK.py 抓取 cookies
    subprocess.run(["python", "getCK_AUTO.py"], check=True)

    # 运行 getSCHL.py 抓取数据
    subprocess.run(["python", "getSCHL.py"], check=True)

if __name__ == "__main__":
    main()

