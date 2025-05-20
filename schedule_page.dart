import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class SchedulePage extends StatefulWidget {
  final String username;

  SchedulePage({required this.username});

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
        Uri.parse('http://localhost:8000/schedule/${widget.username}'),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          _courses = data['courses'];
          _isLoading = false;
        });
      }
    } catch (e) {
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
          BoxShadow(
            color: Colors.blueGrey.withOpacity(0.1),
            blurRadius: 8,
            spreadRadius: 2
          )
        ],
      ),
      child: ListTile(
        leading: Text(
          course['emoji'],
          style: TextStyle(fontSize: 24),
        ),
        title: Text(
          course['name'],
          style: TextStyle(
            fontFamily: 'HarmonyOS Sans',
            fontWeight: FontWeight.w600,
            color: Color(0xFF4A5568)
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '⏰ ${course['time']}',
              style: TextStyle(
                fontFamily: 'Roboto',
                color: Color(0xFF718096)
              ),
            ),
            Text(
              '📍 ${course['location']}',
              style: TextStyle(
                fontFamily: 'Roboto',
                color: Color(0xFF718096)
              ),
            )
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
        title: Text(
          '今日课程',
          style: TextStyle(
            fontFamily: 'HarmonyOS Sans',
            color: Color(0xFF4A5568)
          ),
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          IconButton(
            icon: Icon(Icons.refresh, color: Color(0xFF6C63FF)),
            onPressed: _fetchSchedule,
          )
        ],
      ),
      body: _isLoading
          ? Center(child: CircularProgressIndicator(color: Color(0xFF6C63FF)))
          : ListView.builder(
              itemCount: _courses.length,
              itemBuilder: (context, index) {
                return _buildCourseTile(
                  Map<String, dynamic>.from(_courses[index])
                );
              },
            ),
    );
  }
}

class CourseTile extends StatelessWidget {
  final String emoji;
  final String name;
  final String time;
  final String location;

  // ... existing styling ...
}