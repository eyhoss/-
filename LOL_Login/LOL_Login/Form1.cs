﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace LOL_Login
{
    public partial class Form1 : Form
    {
        [DllImport("ImageSearchDLL.dll")]
        private static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002; // 마우스 왼쪽 버튼 Down
        private const uint MOUSEEVENTF_LEFTUP = 0x0004; // 마우스 오른쪽 버튼 Up

        // 닉네임, 아이디, 비밀번호 저장
        public string[] name = new string[100];
        public string[] id = new string[100];
        public string[] pw = new string[100];

        public void ClickImage(string imgPath)
        {
            int right = Screen.PrimaryScreen.WorkingArea.Right;
            int bottom = Screen.PrimaryScreen.WorkingArea.Bottom;

            IntPtr result = ImageSearch(0, 0, right, bottom, imgPath); // 이미지 서치
            String res = Marshal.PtrToStringAnsi(result); // 결과 변환

            if (res[0] == '0') // 이미지를 찾지 못한 경우
            {
                ClickImage(imgPath);
                return;
            }

            // 화면 배율에 맞게 좌표 변환
            String[] data = res.Split('|'); // 결과 값 나누기
            int x = Convert.ToInt32(data[1]) * 100 / Properties.Settings.Default.scaling;
            int y = Convert.ToInt32(data[2]) * 100 / Properties.Settings.Default.scaling;

            Cursor.Position = new Point(x, y); // 마우스 커서 이동

            //마우스 클릭
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 설정값에서 불러와 배열에 저장
            name = Properties.Settings.Default.name.Split(',');
            id = Properties.Settings.Default.id.Split(',');
            pw = Properties.Settings.Default.pw.Split(',');

            // ComboBox에 입력
            for (int i = 0; i < Properties.Settings.Default.num; i++)
            {
                ComboBox_Select_ID.Items.Add(name[i]);
            }
        }

        private void Button_ID_Click(object sender, EventArgs e)
        {
            // Form2 창 열기
            Form2 newform2 = new Form2(this);
            newform2.ShowDialog();
        }

        private void Button_Settings_Click(object sender, EventArgs e)
        {
            Form3 newform3 = new Form3();
            newform3.ShowDialog();
        }

        private void Button_Help_Click(object sender, EventArgs e)
        {
            Process.Start("https://velog.io/@eyhoss");
        }

        private void Button_Login_Click(object sender, EventArgs e)
        {
            int n = -1;
            // ComboBox에서 닉네임 불러와서 검색
            try
            {
                for (int i = 0; i < Properties.Settings.Default.num; i++)
                    if (name[i] == ComboBox_Select_ID.SelectedItem.ToString())
                        n = i;
            }
            catch (NullReferenceException err)
            {
                RichTextBox_Status.AppendText("아이디를 선택해 주세요.\n");
                return;
            }

            if (n == -1) // 아이디를 찾지 못한 경우
            {
                RichTextBox_Status.AppendText("아이디를 찾을 수 없습니다.\n");
                return;
            }

            // 로그인
            ClickImage("*30 img\\QHD_PW.png");
            SendKeys.Send(pw[n]);
            ClickImage("*30 img\\QHD_ID.png");
            SendKeys.Send(id[n]);
            ClickImage("*50 img\\QHD_LOGIN.png");

            this.Close(); // 프로그램 종료
        }

        private void Button_TurnOn_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\Riot Games\\Riot Client\\RiotClientServices.exe");
        }
    }
}
