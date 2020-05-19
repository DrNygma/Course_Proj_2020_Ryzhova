using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course_Proj_2020_Ryzhova
{
    public partial class Form1 : Form
    {
        public int Numproc = 0;//номер процесса
        public int numpage;//количество страниц процесса, адресующихся в озу
        public int a = 0;//количество страниц процесса
        public List<DelProc> procs = new List<DelProc>();//для хранения информации, нужной для удаления процессов
        public List<Process> procexlist = new List<Process>();//коллекция для хранения информации о процессах
        public Process procex;//создание экземпляра класса, хранящего значения полей создаваемого процесса
        public string[,] ramobjs = new string[21, 2];
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            RAM.RowCount = 21;
            RAM.ColumnCount = 2;
            ramobjs[0, 0] = "Процесс";
            ramobjs[0, 1] = "№ страницы";
            for (int i = 1; i < 21; i++)
            {
                ramobjs[i, 1] = (i).ToString();
            }
            for (int i = 0; i < 21; i++)
            {

                RAM.Rows[i].Cells[0].Value = ramobjs[i, 0];
                RAM.Rows[i].Cells[1].Value = ramobjs[i, 1];
            }
            Proc.AllowUserToAddRows = false;
            VM.AllowUserToAddRows = false;
            Proc.ColumnCount = 3;
            Proc.Rows.Add();
            Proc.Rows[0].Cells[0].Value = "№ вирт. стр.";
            Proc.Rows[0].Cells[1].Value = "№ физ. стр.";
            Proc.Rows[0].Cells[2].Value = "Управл. инф.";
            VM.ColumnCount = 2;
            VM.Rows.Add();
            VM.Rows[0].Cells[0].Value = "№";
            VM.Rows[0].Cells[1].Value = "Процесс №";

        }
        /// <summary>
        /// Кнопка "Создать процесс"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Numproc++;//номер созданного процесса
            comboBox1.Items.Add(Numproc);
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            procex = new Process();
            procex.num = Numproc;
            Random rand = new Random();
            a = rand.Next(4, 10);
            procex.rampg = new int[a];
            procex.vmpg = new int[a];
            procex.rambl = new bool[a];
            //нумерация страниц процесса
            for (int i = 1; i <= a; i++)
            {
                procex.vmpg[i-1] = i;
            }
            //добавление заголовка с номером процесса
            Proc.Rows.Add();
            Proc.Rows[Proc.Rows.Count-1].Cells[1].Value = Numproc.ToString() + " процесс";
            DelProc tmpdel = new DelProc();//создание экземпляра класса для хранения информации о процессе, с целью его удаления
            tmpdel.numb = Proc.Rows.Count - 1;//начало процесса в таблице процессов
            tmpdel.numofpg = a;//количество страниц процесса в таблице процессов
            procs.Add(tmpdel);

            for (int i =1; i <= a; i++)
            {
               Proc.Rows.Add();
               Proc.Rows[Proc.Rows.Count-1].Cells[0].Value = procex.vmpg[i-1].ToString();
            }
            button1.Enabled = false;
            button2.Enabled = true;
        }
        //Выделить память
        private void button2_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button4.Enabled = true;
            numpage = Convert.ToInt32(Math.Floor((numericUpDown1.Value)/4));
            procex.num = Numproc;
            Random rand = new Random();
            procs[procex.num-1].numb1 = VM.Rows.Count - 1;//начало процесса в таблице взу
            procs[procex.num-1].numofpg1 = a-numpage;//количество страниц процесса в таблице взу
                                                  
            if (numpage > a)
            {
                MessageBox.Show("Объем выделяемой памяти больше памяти процесса! Будет выделен максимум.", "Ошибка", MessageBoxButtons.OK);
                numpage = a;
            }   
            //выбор страниц для отправки в взу
            for (int i=1;i<=a-numpage;i++)
            {
                procex.rampg[i-1] = -1;
                procex.rambl[i-1] = false;
                VM.Rows.Add();
                VM.Rows[VM.Rows.Count - 1].Cells[1].Value = procex.num.ToString() + " процесс"+ procex.vmpg[i-1].ToString()+ " страница";
                VM.Rows[VM.Rows.Count - 1].Cells[0].Value = VM.Rows.Count - 1;
            }
            //выбор страниц для отправки в озу
            for (int i = a-numpage+1; i <= a; i++)
            {
                procex.rampg[i-1] = rand.Next(1, 20);
                procex.rambl[i-1] = true;
            }
            for (int i = 1; i <= a; i++)
            {
                if (procex.rambl[i-1] == false)
                    Proc.Rows[Proc.Rows.Count - 1 - a + i].Cells[1].Value = "ВЗУ";
                else
                    Proc.Rows[Proc.Rows.Count - 1 - a + i].Cells[1].Value = procex.rampg[i-1].ToString();
                Proc.Rows[Proc.Rows.Count - 1 - a + i].Cells[2].Value = procex.rambl[i-1].ToString();
            }
            button1.Enabled = true;
            button2.Enabled = false;
            //отправка страниц в озу
            for (int i = a - numpage+1; i <= a; i++)
            {
                if (procex.rambl[i-1] == true)
                {
                    if(RAM.Rows[procex.rampg[i-1]].Cells[0].Value==null)
                        RAM.Rows[procex.rampg[i-1]].Cells[0].Value = procex.num.ToString() + " процесс" + procex.vmpg[i-1].ToString() + " страница";
                    else
                    {
                        bool inram = false;//признак помещения страницы в озу
                        //устранение коллизий, при обращении к уже заполненной странице в озу
                        for (int rt = 1; rt < 99; rt++)
                        {
                            int rnd = rand.Next(1, 21);
                            if (RAM.Rows[rnd].Cells[0].Value == null)
                            {
                                RAM.Rows[rnd].Cells[0].Value = procex.num.ToString() + " процесс" + procex.vmpg[i-1].ToString() + " страница";
                                procex.rampg[i - 1] = rnd;
                                Proc.Rows[procs[procex.num - 1].numb+i].Cells[1].Value = rnd.ToString();
                                inram = true;
                                break;
                            }
                        }  
                        if(inram==false)
                        {
                            MessageBox.Show("ОЗУ заполнено, страница будет направлена в ВЗУ!", "Ошибка", MessageBoxButtons.OK);
                            VM.Rows.Add();
                            VM.Rows[VM.Rows.Count - 1].Cells[1].Value = procex.num.ToString() + " процесс" + procex.vmpg[i-1].ToString() + " страница";
                            VM.Rows[VM.Rows.Count - 1].Cells[0].Value = VM.Rows.Count - 1;
                            procex.rampg[i - 1] = -1;
                            procex.rambl[i - 1] = false;
                            Proc.Rows[procs[procex.num - 1].numb + i].Cells[1].Value = "ВЗУ";
                            Proc.Rows[procs[procex.num - 1].numb + i].Cells[2].Value = procex.rambl[i - 1].ToString();
                            procs[procex.num - 1].numofpg1++;

                        }    
                    }
                }
            }
            procexlist.Add(procex);
        }
        /// <summary>
        /// Кнопка "Удалить процесс"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            int nm = (int)comboBox1.SelectedIndex;
            if(procs[nm].isdel)
            {
                MessageBox.Show("Данный элемент уже удален!");
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                bool islast = false;
                if ((procs[nm].numb + procs[nm].numofpg) == (Proc.Rows.Count-1))//проверка последний ли элемент
                    islast = true;
                if(islast==true)//удаление последнего
                {
                    for (int i = (Proc.Rows.Count - 1); i >= procs[nm].numb; i--)
                    {
                        Proc.Rows.RemoveAt(i);
                    }
                }
                else //удаление непоследнего
                {
                    for (int i = procs[nm].numb + procs[nm].numofpg; i >= procs[nm].numb; i--)
                    {
                        Proc.Rows.RemoveAt(i);
                    }
                }
                //смещение индексов при удалении процесса
                if (islast == false)
                {
                    for (int i = nm+1; i < procs.Count; i++)
                    {
                        procs[i].numb -= procs[nm].numofpg+1;
                        procs[i].numb1 -= procs[nm].numofpg1;
                    }
                }
                //удаление страниц из взу
                for (int i = procs[nm].numb1 + procs[nm].numofpg1; i > procs[nm].numb1; i--)
                    VM.Rows.RemoveAt(i);
                //удаление страниц из озу
                for (int i = 1; i < RAM.Rows.Count; i++)
                {
                    for (int j = 0; j < procexlist[nm].rampg.Length; j++)
                    {
                        if (procexlist[nm].rampg[j] == Convert.ToInt32(RAM.Rows[i].Cells[1].Value))
                            RAM.Rows[i].Cells[0].Value = null;
                    }

                }
                comboBox1.Items[nm] += " удален";
                procs[nm].isdel = true;
            }
        }
        /// <summary>
        /// Кнопка "Выбрать" для свопинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            int unloadpg = Convert.ToInt32(numericUpDown2.Value);
            int loadpg = Convert.ToInt32(numericUpDown3.Value);
            string tmp = RAM.Rows[unloadpg].Cells[0].Value.ToString();
            string VMnumofproc = VM.Rows[loadpg].Cells[1].Value.ToString();
            for (int i = 0; i < procexlist.Count; i++)
            {
                for( int j=0; j<procexlist[i].rampg.Length; j++)
                {
                    if(procexlist[i].rampg[j]==unloadpg)
                    {
                        RAM.Rows[unloadpg].Cells[0].Value = VMnumofproc;
                        int indexofproc;
                        int indexofram;
                        if (VMnumofproc[1] == ' ')
                        {
                            indexofproc = int.Parse(VMnumofproc[0].ToString());
                            indexofram = Convert.ToInt32(VMnumofproc[9].ToString());
                        }
                        else
                        {
                            indexofproc = int.Parse(VMnumofproc[0].ToString())*10+ int.Parse(VMnumofproc[1].ToString());
                            indexofram = Convert.ToInt32(VMnumofproc[9].ToString());
                        }
                        //смещение индексов при загрузке страницы из взу в озу
                        procs[indexofproc - 1].numofpg1--;
                        for (int x=indexofproc;x< procs.Count;x++)
                            procs[x].numb1--;
                        for (int k=loadpg+1;k<VM.Rows.Count;k++)
                            VM.Rows[k-1].Cells[1].Value = VM.Rows[k].Cells[1].Value;
                        //смещение индексов при выгрузке страницы из озу в взу
                        procs[i].numofpg1++;
                        for (int r = i + 1; r < procs.Count; r++)
                            procs[r].numb1++;
                        for(int q=VM.Rows.Count-1;q>= procs[i].numb1+procs[i].numofpg1;q--)
                            VM.Rows[q].Cells[1].Value = VM.Rows[q-1].Cells[1].Value;
                        //замещение выгружаемой и загружаемой страниц
                        VM.Rows[procs[i].numb1 + procs[i].numofpg1].Cells[1].Value = tmp;
                        procexlist[i].rampg[j] = -1;
                        procexlist[i].rambl[j] = false;
                        Proc.Rows[procs[i].numb+j+1].Cells[1].Value = "ВЗУ";
                        Proc.Rows[procs[i].numb + j + 1].Cells[2].Value = procexlist[i].rambl[j].ToString();
                        //запись в коллекцию новых значений для загруженной в озу страницы
                        procexlist[indexofproc - 1].rampg[indexofram - 1] = unloadpg;
                        procexlist[indexofproc - 1].rambl[indexofram - 1] = true;
                        Proc.Rows[procs[indexofproc - 1].numb + indexofram].Cells[1].Value = unloadpg;
                        Proc.Rows[procs[indexofproc - 1].numb + indexofram].Cells[2].Value = procexlist[indexofproc - 1].rambl[indexofram - 1].ToString();
                        break;
                    }
                }
            }
        }
    }
    public class DelProc
    {
        public int numb;//номер строки в таблице, отвечающий за начало процесса в таблице Proc
        public int numb1;//номер строки в таблице, отвечающий за начало процесса в таблице VM
        public int numofpg1;//количество страниц процесса в таблице Proc
        public int numofpg;//количество страниц процесса в таблице в таблице VM
        public bool isdel = false;//признак удаления процесса
    }
    public class Process
    {
        public int num;//номер процесса
        public int[] rampg;//номера страниц процесса 
        public int[] vmpg;//номера физических страниц, в которые были помещены страницы процесса
        public bool[] rambl;//признаки наличия страниц в ОЗУ 
    }
}
