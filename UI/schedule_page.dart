import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:intl/intl.dart';

class SchedulePage extends StatefulWidget {
  final String username;

  const SchedulePage({super.key, required this.username});

  @override
  _SchedulePageState createState() => _SchedulePageState();
}

class _SchedulePageState extends State<SchedulePage> {
  List<dynamic> _courses = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _fetchSchedule();
  }

  Future<void> _fetchSchedule() async {
    setState(() => _isLoading = true);
    try {
      final response = await http.get(
  Uri.parse('http://10.0.2.2:5000/schedule/${widget.username}'),
);

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          _courses = data['courses'];
          _isLoading = false;
        });
      } else {
        setState(() => _isLoading = false);
      }
    } catch (e) {
      print('Error fetching schedule: $e');
      setState(() => _isLoading = false);
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
                Text('${widget.username}', style: TextStyle(fontSize: 16)),
                Text(
// 原代码中 DateFormat 方法是存在的，因为已经导入了 'package:intl/intl.dart'，所以无需修改，直接保留原代码
// 由于 DateFormat 已经正确导入，原代码本身无误，可直接保留
DateFormat('yyyy年M月d日 EEEE', 'zh_CN').format(DateTime.now()),
                  style: TextStyle(fontSize: 12)
                )
              ],
            ),
          ],
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: Icon(Icons.refresh, color: Color(0xFF6C63FF)),
            onPressed: _fetchSchedule,
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
