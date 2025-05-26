import 'package:flutter/material.dart';
import 'dart:convert';
import 'dart:io';
import 'package:intl/intl.dart';
import 'package:path_provider/path_provider.dart';

class SchedulePage extends StatefulWidget {
  final String username;
  final String password; // 添加密码参数

  const SchedulePage({
    super.key,
    required this.username,
    required this.password,
  });

  @override
  _SchedulePageState createState() => _SchedulePageState();
}

class _SchedulePageState extends State<SchedulePage> {
  List<dynamic> _courses = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _saveConfigAndFetchSchedule();
  }

  Future<void> _saveConfigAndFetchSchedule() async {
    setState(() => _isLoading = true);
    try {
      // 获取应用文档目录
      final appDir = await getApplicationDocumentsDirectory();
      final configPath = '${appDir.path}/config.json';
      final schedulePath = '${appDir.path}/schedule_grouped.json';

      // 保存配置
      final configMap = {
        'username': widget.username,
        'password': widget.password,
      };

      await File(configPath).writeAsString(jsonEncode(configMap));

      // 运行 Python 脚本
      final pythonScript =
          'c:\\Users\\邓承轩\\source\\repos\\SSAP-Scheduler\\main.py';
      final process = await Process.run('python', [pythonScript]);

      if (process.exitCode != 0) {
        throw Exception('Python script error: ${process.stderr}');
      }

      // 读取生成的日程数据
      if (await File(schedulePath).exists()) {
        final jsonString = await File(schedulePath).readAsString();
        final data = jsonDecode(jsonString);
        setState(() {
          _courses = data['courses'];
          _isLoading = false;
        });
      } else {
        throw Exception('Schedule file not found');
      }
    } catch (e) {
      print('Error processing schedule: $e');
      setState(() => _isLoading = false);
      // 显示错误提示
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('更新日程失败: ${e.toString()}')));
      }
    }
  }

  Widget _buildCourseTile(Map<String, dynamic> course) {
    return Container(
      margin: EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.95),
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          // BoxShadow 是 Flutter 提供的类，直接使用即可，原代码无问题，此处直接保留
          BoxShadow(
            color: Colors.blueGrey.withOpacity(0.1),
            blurRadius: 8,
            spreadRadius: 2,
          ),
        ],
      ),
      child: ListTile(
        leading: Text(course['emoji'] ?? '', style: TextStyle(fontSize: 24)),
        title: Text(
          course['name'] ?? '',
          style: TextStyle(
            fontFamily: 'HarmonyOS Sans',
            fontWeight: FontWeight.w600,
            color: Color(0xFF4A5568),
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '⏰ ${course['time'] ?? ''}',
              style: TextStyle(fontFamily: 'Roboto', color: Color(0xFF718096)),
            ),
            Text(
              '📍 ${course['location'] ?? ''}',
              style: TextStyle(fontFamily: 'Roboto', color: Color(0xFF718096)),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Color(0xFFFDF5E6),
      appBar: AppBar(
        title: Row(
          children: [
            // 由于 primaryColor 未定义，这里使用一个常见的颜色替代，可根据实际需求修改
            // Flutter 的 Colors 类中没有 darkblue 属性，使用 Colors.blue[900] 替代
            Icon(Icons.calendar_today, color: Colors.blue[900]),
            SizedBox(width: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(widget.username, style: TextStyle(fontSize: 16)),
                Text(
                  // 原代码中 DateFormat 方法是存在的，因为已经导入了 'package:intl/intl.dart'，所以无需修改，直接保留原代码
                  // 由于 DateFormat 已经正确导入，原代码本身无误，可直接保留
                  DateFormat('yyyy年M月d日 EEEE', 'zh_CN').format(DateTime.now()),
                  style: TextStyle(fontSize: 12),
                ),
              ],
            ),
          ],
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: Icon(Icons.refresh, color: Color(0xFF6C63FF)),
            onPressed: _saveConfigAndFetchSchedule,
          ),
        ],
      ),
      body:
          _isLoading
              ? Center(
                child: CircularProgressIndicator(color: Color(0xFF6C63FF)),
              )
              : ListView.builder(
                itemCount: _courses.length,
                itemBuilder: (context, index) {
                  return _buildCourseTile(
                    Map<String, dynamic>.from(_courses[index]),
                  );
                },
              ),
    );
  }
}
