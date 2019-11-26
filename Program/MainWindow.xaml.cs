using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using ZJYC_JSON;
using Algorithm;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Interop;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace WpfApp2
{

    public class DisplayCache
    {
        public List<Entry> DisplayContent = new List<Entry>();
        public int SelectIndex = 0;
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static private ZJSON zJSON = new ZJSON("Temp.json");
        public FilterFormer filterFormer = new FilterFormer("全部");
        public FilterMiddle filterMiddle = new FilterMiddle("全部", "0-0");
        public SorterFormer sorterFormer = new SorterFormer("学习时间");
        public FinderFormer finderFormer = new FinderFormer("");
        public FilterRear filterRear = new FilterRear("全部");
        public SingleChoiceQuestions SingleChoice = new SingleChoiceQuestions(ref zJSON, "顺序");
        public TranslateQuestion GetTranslate = new TranslateQuestion(ref zJSON,"顺序");
        public Relevance relevance = new Relevance(ref zJSON, "顺序");

        public DisplayCache DC = new DisplayCache();

        public void UpdateListView()
        {
            this.Dispatcher.BeginInvoke((Action)delegate () { EntryListView.ItemsSource = null; });

            DC.DisplayContent = zJSON.JsonCacheClone();
            filterFormer.Filter(ref DC.DisplayContent);
            filterMiddle.Filter(ref DC.DisplayContent);
            sorterFormer.Sortor(ref DC.DisplayContent);
            finderFormer.Finder(ref DC.DisplayContent);
            filterRear.Filter(ref DC.DisplayContent);
            ShowCount.Text = DC.DisplayContent.Count.ToString();

            this.Dispatcher.BeginInvoke((Action)delegate () { EntryListView.ItemsSource = DC.DisplayContent; });
        }

        public MainWindow()
        {
            //List<Entry> Temp = new List<Entry>(){new Entry(JP:"398861246",CH:"497311275",CS:"单词" ),};
            //Json.WritToFile(Temp);
            InitializeComponent();

            Filter1.ItemsSource = filterFormer.SupportedFilterMode;
            Filter2.ItemsSource = filterMiddle.SupportedFilterMode;
            Sortor1.ItemsSource = sorterFormer.SupportedSorterMode;

            Filter1.SelectedIndex = 0;
            Filter2.SelectedIndex = 0;
            Sortor1.SelectedIndex = 0;

            SingleChoiceMode.ItemsSource = SingleChoice.SupportedMode;
            SupportedTstType.ItemsSource = SingleChoice.SupportedTstType;
            SupportedDifficulty.ItemsSource = SingleChoice.SupportedDifficulty;

            SingleChoiceMode.SelectedIndex = 0;
            SupportedTstType.SelectedIndex = 0;
            SupportedDifficulty.SelectedIndex = 0;

            TranslateMode.ItemsSource = GetTranslate.SupportedMode;
            TranslateMode.SelectedIndex = 0;

            Sortor2.ItemsSource = filterRear.SupportedFilterMode;
            Sortor2.SelectedIndex = 0;

            ImportType.ItemsSource = NewOneClass.ItemsSource = new string[] { "单词","语法","翻译"};

            ImportType.SelectedIndex = NewOneClass.SelectedIndex = 0;

            RelevanceMode.ItemsSource = relevance.SupportedMode;
            RelevanceMode.SelectedIndex = 0;

            UpdateListView();

            //SortMode.AddHandler(ComboBox.MouseDownEvent, new RoutedEventHandler(SortMode_MouseDown), true);
        }

        private void AddNewEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CurJP.Text!=string.Empty && CurCH.Text != string.Empty)
            {
                Entry entry = new Entry(JP: CurJP.Text,CH: CurCH.Text,CS:(string)NewOneClass.SelectedValue);
                zJSON.EntryAddTOCache(entry);
                zJSON.WritJsonFileFrCache();
                UpdateListView();
            }
        }

        private void ModifyEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CurJP.Text != string.Empty && CurCH.Text != string.Empty)
            {
                Entry entry = new Entry(JP: CurJP.Text, CH: CurCH.Text, CS: (string)NewOneClass.SelectedValue);
                int ID = DC.DisplayContent[DC.SelectIndex].PM.ID;
                zJSON.EntryModifyInCache(ID, entry);
                zJSON.WritJsonFileFrCache();
                UpdateListView();
            }
        }

        private void EntryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(EntryListView.SelectedValue != null)
            {
                DC.SelectIndex = EntryListView.SelectedIndex;
                CurJP.Text = DC.DisplayContent[DC.SelectIndex].JP;
                CurCH.Text = DC.DisplayContent[DC.SelectIndex].CH;
            }
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CurJP.Text != string.Empty && CurCH.Text != string.Empty)
            {
                zJSON.EntryDeleteIncache(DC.DisplayContent[DC.SelectIndex].PM.ID);
                UpdateListView();
            }
        }

        private void BeginOrNext_Click(object sender, RoutedEventArgs e)
        {
            SelectResult.Text = "";

            //SingleChoice.ImportEntries(ref DC.DisplayContent, SingleChoiceMode.Text);
            SingleChoice.Mode = (string)SingleChoiceMode.SelectedValue;
            SingleChoice.TstType = (string)SupportedTstType.SelectedValue;
            SingleChoice.Difficulty = (string)SupportedDifficulty.SelectedValue;
            SingleChoice.GenOneQuestion(zJSON.JsonCache);

            Selection1Text.Text = SingleChoice.Out.SelectionA;
            Selection2Text.Text = SingleChoice.Out.SelectionB;
            Selection3Text.Text = SingleChoice.Out.SelectionC;
            Selection4Text.Text = SingleChoice.Out.SelectionD;
            QuestionText.Text = SingleChoice.Out.Title;
            SingleChoiceRemainCount.Text = SingleChoice.Remain().ToString();

        }

        private void SelectJudgeCH(string CH)
        {
            if(SingleChoice.Judging(CH) == true)
            {
                SelectResult.Text = "选择正确";
            }
            else
            {
                SelectResult.Text = string.Format("选择错误，正确结果为：" + SingleChoice.Out.RightSelet);
            }
        }



        private void Selection1_Click(object sender, RoutedEventArgs e)
        {
            SelectJudgeCH(Selection1Text.Text);
        }

        private void Selection2_Click(object sender, RoutedEventArgs e)
        {
            SelectJudgeCH(Selection2Text.Text);
        }

        private void Selection3_Click(object sender, RoutedEventArgs e)
        {
            SelectJudgeCH(Selection3Text.Text);
        }

        private void Selection4_Click(object sender, RoutedEventArgs e)
        {
            SelectJudgeCH(Selection4Text.Text);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTableControl.SelectedIndex == 1) SingleChoice.ImportEntries(ref DC.DisplayContent, (string)SingleChoiceMode.SelectedValue);
            if (MainTableControl.SelectedIndex == 2) GetTranslate.ImportEntries(ref DC.DisplayContent,(string)TranslateMode.SelectedValue);
            if (MainTableControl.SelectedIndex == 4) relevance.ImportEntries(ref DC.DisplayContent, (string)RelevanceMode.SelectedValue);
            UpdateListView();
        }

        private void Filter1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterFormer.FilterMode = (string)Filter1.SelectedValue;
            UpdateListView();
        }

        private void Filter2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterMiddle.FilterMode = (string)Filter2.SelectedValue;
            filterMiddle.FilterParam = SortConfigParam.Text;
            UpdateListView();
        }

        private void Sortor1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sorterFormer.SorterMode = (string)Sortor1.SelectedValue;
            UpdateListView();
        }

        private void KeyWordInput_Click(object sender, RoutedEventArgs e)
        {
            finderFormer.Keyword = KeywordInput.Text;
            UpdateListView();
        }

        private void OpenJPFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                JPFileName.Text = fileDialog.FileName;
            }
        }

        private void OpenCHFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                CHFileName.Text = fileDialog.FileName;
            }
        }

        private void ImportNewEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CHFileName.Text != "" && JPFileName.Text != "")
            {
                List<string> JPContent = new List<string>();
                List<string> CHContent = new List<string>();

                using (StreamReader ReaderJP = new StreamReader(JPFileName.Text))
                {
                    String Input;
                    while ((Input = ReaderJP.ReadLine()) != null)
                    {
                        JPContent.Add(Input);
                    }
                    ReaderJP.Close();
                }

                using (StreamReader ReaderCH = new StreamReader(CHFileName.Text))
                {
                    String Input;
                    while ((Input = ReaderCH.ReadLine()) != null)
                    {
                        CHContent.Add(Input);
                    }
                    ReaderCH.Close();
                }

                if (JPContent.Count != CHContent.Count)
                {
                    MessageBox.Show("两个文件不一致");
                }
                else
                {
                    MessageBox.Show("一共有"+ JPContent.Count.ToString()+"条");
                }

                List<Entry> Entries = new List<Entry>();
                for(int i = 0;i < JPContent.Count; i++)
                {
                    Entries.Add(new Entry(JP: JPContent[i], CH: CHContent[i], CS: (string)ImportType.SelectedValue));
                }

                zJSON.EntryAddTOCache(Entries);
                zJSON.WritJsonFileFrCache();
                UpdateListView();
            }
        }

        private void OpenTheDic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                DIC_Path.Text = fileDialog.FileName;
            }
        }

        private void DIC_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            zJSON.WritJsonFileFrCache();
            zJSON.SetJsonFileName(DIC_Path.Text);
            zJSON.ReadJsonFileToCache();
            UpdateListView();
        }

        private void CreateJsonFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "ext files (*.json)|*.json|All files(*.*)|*>**";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true && saveFileDialog1.FileName.Length > 0)
            {
                NewJsonFileName.Text = saveFileDialog1.FileName;
                List<Entry> Temp = new List<Entry>()
                {
                    new Entry(JP:"A new",CH:"A new",CS:"单词" ),
                };
                zJSON.WritJsonFileFrCache();
                zJSON.SetJsonFileName(NewJsonFileName.Text);
                zJSON.FillJsonFileWsBlank();
                zJSON.ReadJsonFileToCache();
                UpdateListView();
            }
        }

        private void SingleChoiceMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SingleChoice.Mode = SingleChoiceMode.Text;
        }

        private void SupportedTstType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SingleChoice.TstType = (string)SupportedTstType.SelectedValue;
        }

        private void SupportedDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SingleChoice.Difficulty = (string)SupportedDifficulty.SelectedValue;
        }

        private void TranslateBeginOrNext_Click(object sender, RoutedEventArgs e)
        {
            TranslateScore.Text = "";
            TranslateQuestion.Text = "";
            TranslateUserAnswer.Text = "";
            TranslateScore.Text = "";
            TranslateRightAnswer.Text = "";

            GetTranslate.Mode = (string)TranslateMode.SelectedValue;
            GetTranslate.GenOneQuestion();
            TranslateQuestion.Text = GetTranslate.Out.Question;
        }

        private void SubmitTheAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (TranslateUserAnswer.Text == "") MessageBox.Show("请先输入答案");
            TranslateScore.Text = GetTranslate.Judging(TranslateUserAnswer.Text).ToString("F3");
            TranslateRightAnswer.Text = GetTranslate.Out.RightAnswer;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Filter = "ext files (*.txt)|*.txt|All files(*.*)|*>**";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true && saveFileDialog1.FileName.Length > 0)
            {
                string FileName = saveFileDialog1.FileName;
                try
                {
                    using (StreamWriter Writer = new StreamWriter(FileName))
                    {
                        foreach (Entry entry in DC.DisplayContent)
                        {
                            string Str1 = entry.JP;
                            string Str2 = entry.CH;
                            Str1 = Str1.Replace("<","\r\n<");
                            Str2 = Str2.Replace("[","\r\n[");
                            Writer.WriteLine(Str1);
                            Writer.WriteLine(Str2);
                            Writer.WriteLine("-------------------------------------------------------");
                        }
                    }
                    MessageBox.Show("成功保存到文件：" + FileName);
                }
                catch
                {
                    MessageBox.Show("保存文件出错了");
                }
            }
        }

        private void Sortor2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterRear.FilterMode = (string)Sortor2.SelectedValue;
            filterRear.FilterParam = FilterRearParam.Text;
            UpdateListView();
        }

        private void RelevanceMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            relevance.Mode = (string)RelevanceMode.SelectedValue;
        }

        private void RelevanceNext_Click(object sender, RoutedEventArgs e)
        {
            RelevanceAnswer1.Text = "";
            RelevanceAnswer2.Text = "";
            RelevanceAnswer3.Text = "";
            RelevanceAnswer4.Text = "";
            RelevanceAnswer5.Text = "";
            RelevanceAnswer6.Text = "";
            RelevanceQuestion.Text = "";

            relevance.Mode = (string)RelevanceMode.SelectedValue;
            relevance.GenOneQuestion(zJSON.JsonCache);

            RelevanceAnswer1.Text = relevance.Out.RightAnswer[0].JP + "\r\n" + relevance.Out.RightAnswer[0].CH;
            RelevanceAnswer2.Text = relevance.Out.RightAnswer[1].JP + "\r\n" + relevance.Out.RightAnswer[1].CH;
            RelevanceAnswer3.Text = relevance.Out.RightAnswer[2].JP + "\r\n" + relevance.Out.RightAnswer[2].CH;
            RelevanceAnswer4.Text = relevance.Out.RightAnswer[3].JP + "\r\n" + relevance.Out.RightAnswer[3].CH;
            RelevanceAnswer5.Text = relevance.Out.RightAnswer[4].JP + "\r\n" + relevance.Out.RightAnswer[4].CH;
            RelevanceAnswer6.Text = relevance.Out.RightAnswer[5].JP + "\r\n" + relevance.Out.RightAnswer[5].CH;
            RelevanceQuestion.Text = relevance.Out.Question.JP + "\r\n" + relevance.Out.Question.CH;

            RelevanceRemain.Text = relevance.Remain().ToString();

        }
    }
    
}
