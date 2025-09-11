# -*- coding: utf-8 -*-
import subprocess
import os
import sys
import json

def run_executable(exe_name, py_fallback=None):
    """运行可执行文件，如果失败则尝试Python脚本"""
    try:
        subprocess.run([exe_name], check=True)
        return True
    except (subprocess.CalledProcessError, FileNotFoundError) as e:
        print(f"可执行文件 {exe_name} 执行失败: {e}")
        
        if py_fallback and os.path.exists(py_fallback):
            print(f"尝试运行 Python 脚本: {py_fallback}")
            try:
                subprocess.run([sys.executable, py_fallback], check=True)
                return True
            except subprocess.CalledProcessError as py_e:
                print(f"Python 脚本也执行失败: {py_e}")
                return False
        else:
            print(f"未找到备用 Python 脚本: {py_fallback}")
            return False

def get_cookies():
    """获取 cookies，优先使用 exe，失败时使用 Python 脚本"""
    return run_executable("getCK_new.exe", "getCK_new.py")

def get_schedule():
    """获取课表数据，优先使用 exe，失败时使用 Python 脚本"""
    return run_executable("getSCHL.exe", "getSCHL.py")

def main():
    print("开始获取 cookies...")
    if not get_cookies():
        print("❌ 获取 cookies 失败，程序终止")
        return False
    
    print("开始获取课表数据...")
    if not get_schedule():
        print("❌ 获取课表数据失败，程序终止")
        return False
    
    print("✅ 所有任务完成成功")
    return True

if __name__ == "__main__":
    main()
