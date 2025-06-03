# -*- coding: utf-8 -*-
import subprocess
import os
import sys

def run_executable(exe_name):
    
    
    try:
        subprocess.run([exe_name], check=True)
    except subprocess.CalledProcessError as e:
        print(f"执行失败: {e}")
        raise

def get_cookies():
    """运行 getCK_AUTO.exe 抓取 cookies"""
    run_executable("getCK_AUTO.exe")

def get_schedule():
    """运行 getSCHL.exe 抓取课表数据"""
    run_executable("getSCHL.exe")

def main():
    # 调用可执行文件
    get_cookies()
    get_schedule()

if __name__ == "__main__":
    main()
