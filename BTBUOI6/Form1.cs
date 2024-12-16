using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace BTBUOI6
{
    public partial class Form1 : Form
    {
       private Model1 _context;
        public Form1()
        {
            InitializeComponent();
            _context = new Model1();
            LoadFaculties();
            LoadStudents();

        }
        private void LoadFaculties()
        {

            var faculties = _context.Faculties.ToList();
            cbbKhoa.DataSource = faculties;
            cbbKhoa.DisplayMember = "FacultyName";
            cbbKhoa.ValueMember = "FacultyID";
        }
        private void LoadStudents()
        {

            var students = _context.Students
                                   .Include(s => s.Faculty)
                                   .Select(s => new
                                   {
                                       s.StudentID,
                                       s.FullName,
                                       FacultyName = s.Faculty.FacultyName,
                                       s.AverageScore
                                      

                                   })
                                   .ToList();


            dataGridView1.Rows.Clear();
            foreach (var student in students)
            {
                dataGridView1.Rows.Add(student.StudentID, student.FullName, student.FacultyName, student.AverageScore);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMSSV.Text) || string.IsNullOrWhiteSpace(txtHoten.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK,
                                                                                    MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtDTB.Text, out var average) || average < 0 || average > 10)
                {
                    MessageBox.Show("Bạn đã nhập sai kiểu dữ liệu ! xin hãy nhập lại!", "Thông báo",
                                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var student = new Student
                {
                    StudentID = txtMSSV.Text,
                    FullName = txtHoten.Text,
                    FacultyID = (int)cbbKhoa.SelectedValue,
                    AverageScore = average
                };

                _context.Students.Add(student);
                _context.SaveChanges();
                MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK,
                                                                        MessageBoxIcon.Information);
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK,
                                                                    MessageBoxIcon.Error);
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {

                var studentID = txtMSSV.Text;
                var student = _context.Students.FirstOrDefault(s => s.StudentID == studentID);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                student.FullName = txtHoten.Text;
                if (double.TryParse(txtDTB.Text, out var average) && average >= 0 && average <= 10)
                {
                    student.AverageScore = average;
                }
                else
                {
                    MessageBox.Show("Bạn đã nhập sai kiểu dữ liệu! Vui lòng nhập điểm từ 0 đến 10.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                student.FacultyID = (int)cbbKhoa.SelectedValue;
                _context.SaveChanges();
                MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var studentID = txtMSSV.Text;
                var student = _context.Students.FirstOrDefault(s => s.StudentID == studentID);

                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa!", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _context.Students.Remove(student);
                    _context.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK,
                                                                        MessageBoxIcon.Information);
                    LoadStudents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK,
                                                                    MessageBoxIcon.Error);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                var row = dataGridView1.Rows[e.RowIndex];
                txtMSSV.Text = row.Cells[0].Value.ToString();
                txtHoten.Text = row.Cells[1].Value.ToString();
                cbbKhoa.Text = row.Cells[2].Value.ToString();
                txtDTB.Text = row.Cells[3].Value.ToString();
                

            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadStudents();
        }
    }
}
