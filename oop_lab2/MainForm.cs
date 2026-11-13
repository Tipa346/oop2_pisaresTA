using System;
using System.Drawing;
using System.Windows.Forms;
using Lab2;

namespace Lab2
{
    public class MainForm : Form
    {
        private readonly Drawing _drawing = new Drawing();

        private Panel panelCanvas;

        private Button btnLine;
        private Button btnRect;
        private Button btnEllipse;
        private Button btnPencil;

        private Button btnColor;
        private Button btnUndo;
        private Button btnClear;
        private Button btnSave;
        private Button btnOpen;

        private NumericUpDown numWidth;

        private string _currentTool = "Line";

        private Color _currentColor = Color.Black;

        private int _currentWidth = 2;

        private Shape _currentShape;

        private bool _isDrawing = false;

        public MainForm()
        {
            Text = "Paint — графический редактор";

            Size = new Size(1000, 700);

            MinimumSize = new Size(800, 500);

            StartPosition = FormStartPosition.CenterScreen;

            KeyPreview = true;

            CreateInterface();

            SelectTool("Line");
        }

        // Создание интерфейса
        private void CreateInterface()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel();

            mainLayout.Dock = DockStyle.Fill;

            mainLayout.RowCount = 3;

            mainLayout.ColumnCount = 1;

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Controls.Add(mainLayout);

            // Верхняя панель
            FlowLayoutPanel toolPanel = new FlowLayoutPanel();

            toolPanel.Dock = DockStyle.Fill;

            toolPanel.WrapContents = false;

            toolPanel.Padding = new Padding(10);

            mainLayout.Controls.Add(toolPanel, 0, 0);

            btnLine = CreateButton("Линия", 100);
            btnRect = CreateButton("Прямоугольник", 130);
            btnEllipse = CreateButton("Эллипс", 100);
            btnPencil = CreateButton("Карандаш", 110);

            btnLine.Click += (s, e) => SelectTool("Line");
            btnRect.Click += (s, e) => SelectTool("Rect");
            btnEllipse.Click += (s, e) => SelectTool("Ellipse");
            btnPencil.Click += (s, e) => SelectTool("Pencil");

            toolPanel.Controls.Add(btnLine);
            toolPanel.Controls.Add(btnRect);
            toolPanel.Controls.Add(btnEllipse);
            toolPanel.Controls.Add(btnPencil);

            btnColor = CreateButton("Цвет", 75);

            btnColor.Click += BtnColor_Click;

            toolPanel.Controls.Add(btnColor);

            Label lblWidth = new Label();

            lblWidth.Text = "Толщина:";

            lblWidth.AutoSize = true;

            lblWidth.Margin = new Padding(10, 10, 0, 0);

            toolPanel.Controls.Add(lblWidth);

            numWidth = new NumericUpDown();

            numWidth.Minimum = 1;
            numWidth.Maximum = 20;
            numWidth.Value = 2;

            numWidth.Width = 55;

            numWidth.Margin = new Padding(5);

            numWidth.ValueChanged += (s, e) =>
            {
                _currentWidth = (int)numWidth.Value;
            };

            toolPanel.Controls.Add(numWidth);

            // Вторая панель
            FlowLayoutPanel actionPanel = new FlowLayoutPanel();

            actionPanel.Dock = DockStyle.Fill;

            actionPanel.WrapContents = false;

            actionPanel.Padding = new Padding(10, 5, 10, 5);

            mainLayout.Controls.Add(actionPanel, 0, 1);

            btnUndo = CreateButton("Отменить", 110);
            btnClear = CreateButton("Очистить", 110);
            btnOpen = CreateButton("Открыть", 110);
            btnSave = CreateButton("Сохранить", 110);

            btnUndo.Click += BtnUndo_Click;
            btnClear.Click += BtnClear_Click;
            btnOpen.Click += BtnOpen_Click;
            btnSave.Click += BtnSave_Click;

            actionPanel.Controls.Add(btnUndo);
            actionPanel.Controls.Add(btnClear);

            // Пустое пространство перед кнопками открытия и сохранения
            Label spacer = new Label();

            spacer.AutoSize = false;

            spacer.Width = 300;

            actionPanel.Controls.Add(spacer);

            actionPanel.Controls.Add(btnOpen);
            actionPanel.Controls.Add(btnSave);

            // Холст
            panelCanvas = new Panel();

            panelCanvas.Dock = DockStyle.Fill;

            panelCanvas.BackColor = Color.White;

            panelCanvas.BorderStyle = BorderStyle.FixedSingle;

            panelCanvas.Margin = new Padding(10);

            panelCanvas.Paint += PanelCanvas_Paint;

            panelCanvas.MouseDown += PanelCanvas_MouseDown;
            panelCanvas.MouseMove += PanelCanvas_MouseMove;
            panelCanvas.MouseUp += PanelCanvas_MouseUp;

            mainLayout.Controls.Add(panelCanvas, 0, 2);
        }

        // Создание кнопки
        private Button CreateButton(string text, int width)
        {
            Button button = new Button();

            button.Text = text;

            button.Width = width;

            button.Height = 35;

            button.Margin = new Padding(3);

            return button;
        }

        // Выбор инструмента
        private void SelectTool(string tool)
        {
            _currentTool = tool;

            btnLine.BackColor = SystemColors.Control;
            btnRect.BackColor = SystemColors.Control;
            btnEllipse.BackColor = SystemColors.Control;
            btnPencil.BackColor = SystemColors.Control;

            switch (tool)
            {
                case "Line":
                    btnLine.BackColor = Color.LightBlue;
                    break;

                case "Rect":
                    btnRect.BackColor = Color.LightBlue;
                    break;

                case "Ellipse":
                    btnEllipse.BackColor = Color.LightBlue;
                    break;

                case "Pencil":
                    btnPencil.BackColor = Color.LightBlue;
                    break;
            }
        }

        // Выбор цвета
        private void BtnColor_Click(object sender, EventArgs e)
        {
            using ColorDialog dialog = new ColorDialog();

            dialog.Color = _currentColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _currentColor = dialog.Color;

                btnColor.BackColor = _currentColor;
            }
        }

        // Нажатие мыши
        private void PanelCanvas_MouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _currentShape = ShapeFactory.Create(
                _currentTool,
                e.Location,
                _currentColor,
                _currentWidth
            );

            _isDrawing = true;

            panelCanvas.Invalidate();
        }

        // Движение мыши
        private void PanelCanvas_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (!_isDrawing || _currentShape == null)
                return;

            _currentShape.MoveEnd(e.Location);

            panelCanvas.Invalidate();
        }

        // Отпускание мыши
        private void PanelCanvas_MouseUp(
            object sender,
            MouseEventArgs e)
        {
            if (!_isDrawing || e.Button != MouseButtons.Left)
                return;

            _currentShape.MoveEnd(e.Location);

            _drawing.Add(_currentShape);

            _currentShape = null;

            _isDrawing = false;

            panelCanvas.Invalidate();
        }

        // Отрисовка холста
        private void PanelCanvas_Paint(
            object sender,
            PaintEventArgs e)
        {
            _drawing.Draw(e.Graphics);

            if (_currentShape != null)
            {
                _currentShape.Draw(e.Graphics);
            }
        }

        // Отмена последнего действия
        private void BtnUndo_Click(object sender, EventArgs e)
        {
            _drawing.Undo();

            panelCanvas.Invalidate();
        }

        // Очистка
        private void BtnClear_Click(object sender, EventArgs e)
        {
            _drawing.Clear();

            _currentShape = null;

            _isDrawing = false;

            panelCanvas.Invalidate();
        }

        // Сохранение
        private void BtnSave_Click(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "JSON файлы (*.json)|*.json";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _drawing.Save(dialog.FileName);

                    MessageBox.Show("Рисунок сохранён!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Открытие
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "JSON файлы (*.json)|*.json";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _drawing.Load(dialog.FileName);

                    _currentShape = null;

                    _isDrawing = false;

                    panelCanvas.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Ctrl + Z
        protected override bool ProcessCmdKey(
            ref Message msg,
            Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Z))
            {
                _drawing.Undo();

                panelCanvas.Invalidate();

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}