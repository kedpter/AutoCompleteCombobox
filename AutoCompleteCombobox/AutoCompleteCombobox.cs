using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Microsoft.International.Converters.PinYinConverter;
using System.Text.RegularExpressions;
using System.Threading;

namespace WanJi.Common.Forms.Controls
{
    public partial class AutoCompleteCombobox : System.Windows.Forms.ComboBox
    {
        public override int SelectedIndex
        {
            get
            {
                if (Items.Count > 0)
                    return base.SelectedIndex;
                else
                    return -1;
            }
            set
            {
                base.SelectedIndex = value;

                //if (Items.Count > 0)
                //{
                //}
            }
        }

        private IFilter filter;
        private IFilter fuzzyfilter;


        public bool EnableFuzzyMatch { get; set; }
          
        public AutoCompleteCombobox()
        {
            Enter += new System.EventHandler(ComboBoxEx_Enter);
            Leave += new System.EventHandler(ComboBoxEx_Leave);
            TextUpdate += new System.EventHandler(ComboBoxEx_TextUpdate);

            DrawMode = DrawMode.OwnerDrawVariable;
            DrawItem += new DrawItemEventHandler(ComboBoxEx_DrawItem);

            EnableFuzzyMatch = false;
            filter = new PinyinFirstLetterFilter(new PinyinFullLetterFilter(new ChineseFilter(new EnglishFilter(new DefaultFilter()))));
            fuzzyfilter = new FuzzyMatchFilter(new DefaultFilter());
            this.DropDownHeight = 140;

        }
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            ComboBox cb = (ComboBox)this;
            if (cb.DataSource == null)
            {
                return;
            }

            lstItems.Clear();
            var t = cb.DataSource as IEnumerable<object>;
            if (t != null)
            {
                foreach (var item in t)
                {
                    lstItems.Add(item);
                }
            }
            //lstItems.AddRange(cb.Items);
        }



        private void AutoCompleteCombobox_DataSourceChanged(object sender, EventArgs e)
        {
            
        }

        #region 自定义绘制每项的颜色
        private void ComboBoxEx_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (e.Index != -1)
            {
                cb.DrawMode = DrawMode.OwnerDrawVariable;
                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Define the default color of the brush as black.
                Brush myBrush = new SolidBrush(this.ForeColor);

                // Determine the color of the brush to draw each item based on the index of the item to draw.
                //if (e.Index % 2 == 0)
                //{
                //    myBrush = Brushes.DarkSlateGray;
                //}
                // Draw background color for each item.
                //e.Graphics.FillRectangle(myBrush, e.Bounds);

                // Draw the current item text based on the current Font and the custom brush settings.
                var diplay = GetDisplayString(cb.Items[e.Index], DisplayMember);
                e.Graphics.DrawString(diplay.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }
        }
        #endregion

        #region 事件

        private ArrayList lstItems = new ArrayList();

        private void ComboBoxEx_Enter(object sender, EventArgs e)
        {
            //ComboBox cb = (ComboBox)sender;
            //lstItems.Clear();
            //lstItems.AddRange(cb.Items);
        }

        private void ComboBoxEx_Leave(object sender, EventArgs e)
        {
            //ComboBox cb = (ComboBox)sender;
            //object obj = null;

            //if (cb.Items.Count != 0)
            //    obj = cb.SelectedItem;

            ////cb.DataSource = null;
            //EmptyDataSource(cb);
            //cb.Items.Clear();
            //cb.DataSource = (ArrayList)lstItems.Clone();
            //cb.SelectedItem = obj;
        }

        private void EmptyDataSource(ComboBox cb)
        {
            _displayMember = cb.DisplayMember;
            _valueMember = cb.ValueMember;

            cb.DataSource = null;

            cb.DisplayMember = _displayMember;
            cb.ValueMember = _valueMember;
        }

        private string _displayMember;
        private string _valueMember;

        private void ComboBoxEx_TextUpdate(object sender, EventArgs e)
        {
            try
            {
              ComboBox cb = (ComboBox)sender;
                var currentFilter = EnableFuzzyMatch ? fuzzyfilter : filter;

                string userInput = cb.Text;
                //cb.DataSource = null;
                EmptyDataSource(cb);

                //if (cb.Items.Count == 0)
                //{
                //    cb.SelectedIndex = -1;
                //    cb.SelectedItem = null;
                //}

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    cb.Items.Clear();
                    cb.Items.AddRange(lstItems.ToArray());
                }
                else
                {
                    ArrayList lstCurItems = new ArrayList();

                    int count = userInput.Length - 1;
                    if (0 == count)
                        lstCurItems = lstItems;
                    else
                        lstCurItems.AddRange(cb.Items);

                    cb.Items.Clear();

                    List<object> items = lstCurItems.Cast<object>().ToList();

                    //object a = items.FirstOrDefault();
                    //var v = a.GetType().GetProperty(this.DisplayMember).GetValue(a, null);

                    var results = currentFilter.Filter(items, DisplayMember, userInput);

                    cb.Items.AddRange(results.ToArray());

                }

                if (cb.Items.Count == 0)
                {
                    cb.SelectedIndex = -1;
                    cb.SelectedItem = null;
                    cb.Text = null;
                    cb.Items.AddRange(lstItems.ToArray());

                    cb.DroppedDown = true;
                    cb.SelectedIndex = -1;

                    cb.Cursor = Cursors.Default;
                    cb.SelectionStart = cb.Text.Length;
                }
                else
                {
                    cb.DroppedDown = true;
                    cb.SelectedIndex = -1;

                    cb.Text = userInput;

                    cb.Cursor = Cursors.Default;
                    cb.SelectionStart = cb.Text.Length;
                    //cb.DropDownHeight = 140;
                }
               
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        private static string GetDisplayString(object obj, string _displayMember)
        {
            return obj.GetType().GetProperty(_displayMember).GetValue(obj, null).ToString();
        }


        #region Filter
        public abstract class IFilter
        {
            public IFilter _successor { get; }


            public IFilter()
            {

            }

            public IFilter(IFilter filter)
            {
                _successor = filter;

            }

            public List<object> Filter(List<object> unfilteredData, string _displayMember, string userInput)
            {
                var results = _Filter(unfilteredData, _displayMember,  userInput);
                if (results == null)
                {
                    // end of chain
                    return new List<object>();
                }
                else
                {
                    var remaining = unfilteredData.Except(results).ToList();
                    if (remaining.Count > 0)
                    {
                        //var sr = _successor.Filter(remaining, userInput);
                        //results.AddRange(sr);
                        results.AddRange(_successor.Filter(remaining, _displayMember, userInput));
                    }
                }

                return results;
            }

            /// <summary>
            /// 过滤出匹配userInput的选项
            /// </summary>
            /// <param name="unfilteredData"></param>
            /// <param name="userInput"></param>
            /// <returns></returns>
            protected abstract List<object> _Filter(List<object> unfilteredData, string _displayMember, string userInput);

            protected string GetFilterString(object obj, string _displayMember)
            {
                return obj.GetType().GetProperty(_displayMember).GetValue(obj, null).ToString();
            }
        }

        public class PinyinFirstLetterFilter : IFilter
        {
            public PinyinFirstLetterFilter(IFilter filter) : base(filter)
            {
            }

            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                int count = userInput.Length - 1;

                List<object> results = new List<object>();
                string userLetters = userInput.ToLower();

                foreach (var obj in _unfilteredData)
                {
                    var itemLetters = GetFilterString(obj, _displayMember).ToLower();
                    if (count < itemLetters.Length)
                    {
                        char currentLetter = itemLetters[count];
                        //Match isch = Regex.Match(currentLetter.ToString(), @"[\u4e00-\u9fa5]");
                        // 如果是中文
                        if (CharUtil.IsChineseLetter(currentLetter))
                        {
                            // (首字母)
                            // c = zhong
                            // 注： pinyin 有多种发音的，每一种都可以
                            List<string> pinyins = PinyinUtil.Chinese2Pinyin(currentLetter);

                            foreach (var pinyin in pinyins)
                            {
                                if (pinyin.ToLower()[0] == userLetters[count])
                                {
                                    results.Add(obj);
                                    // 找到匹配的了
                                    break;
                                }
                            }

                            continue;


                        }

                    }
                }
                return results;
            }
        }



        public class PinyinFullLetterFilter : IFilter
        {
            public PinyinFullLetterFilter(IFilter filter) : base(filter)
            {
            }

            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                int last = userInput.Length - 1;

                List<object> results = new List<object>();
                userInput = userInput.ToLower();

                foreach (var obj in _unfilteredData)
                {
                    var itemLetters = GetFilterString(obj, _displayMember).ToLower();

                    // 一个一个字符比较
                    // e.g. : itemLetters = zz荣智 userInput = zzrongz
                    int ptr_user = 0;

                    bool isInputMatch = true;

                    for (int ptr_sc = 0; ptr_sc < itemLetters.Count(); ptr_sc++)
                    {
                        //if (CharUtil.IsEnglishLetter(itemLetters[ptr_sc]) && userInput[ptr_sc] == itemLetters[ptr_sc])
                        //{
                        //    ptr_user++;
                        //    continue;
                        //}
                        if (ptr_user > last)
                        {
                            break;
                        }
                        if (CharUtil.IsChineseLetter(itemLetters[ptr_sc]))
                        {
                            List<string> pinyins = PinyinUtil.Chinese2Pinyin(itemLetters[ptr_sc]);
                            bool anyPinyinMatch = false;
                            foreach (var pinyin in pinyins)
                            {
                                int sub = pinyin.Length < userInput.Length - ptr_user ? pinyin.Length : userInput.Length - ptr_user;
                                if (pinyin.Substring(0, sub) == userInput.Substring(ptr_user, sub))
                                {
                                    ptr_user += sub;
                                    anyPinyinMatch = true;
                                    break;
                                }
                            }

                            if (!anyPinyinMatch)
                            {
                                isInputMatch = false;
                                break;
                            }
                        }
                        else if (userInput[ptr_user] == itemLetters[ptr_sc])
                        {
                            ptr_user++;
                            continue;
                        }
                        else
                        {
                            isInputMatch = false;
                            ptr_user++;
                            break;
                        }
                    }

                    // 用户输入多余字符， 认为不匹配
                    if (ptr_user <= last)
                    {
                        isInputMatch = false;
                    }

                    if (isInputMatch)
                    {
                        results.Add(obj);
                    }

                    //char currentLetter = itemLetters[last];
                    ////Match isch = Regex.Match(currentLetter.ToString(), @"[\u4e00-\u9fa5]");
                    //// 如果是中文
                    //if (CharUtil.IsChineseLetter(currentLetter))
                    //{
                    //    // (首字母)
                    //    // c = zhong
                    //    // 注： pinyin 有多种发音的，每一种都可以
                    //    List<string> pinyins = PinyinUtil.Chinese2Pinyin(currentLetter);

                    //    foreach (var pinyin in pinyins)
                    //    {
                    //        if (pinyin.ToLower()[0] == userLetters[last])
                    //        {
                    //            results.Add(obj);
                    //            // 找到匹配的了
                    //            break;
                    //        }
                    //    }

                    //    continue;


                    //}

                }
                return results;
            }
            //// 全拼
            //foreach (var pinyin in pinyins)
            //{
            //    var enletters = Regex.Replace(pinyin, @"[^a-zA-Z]", string.Empty).ToLower();
            //    int pos = enletters.IndexOf(userLetters[count]);
            //    if (pos != -1)
            //    {
            //        string sub = enletters.Substring(0, pos + 1);
            //        if (userLetters.Contains(sub))
            //        {
            //            cb.Items.Add(itemLetters);
            //            // 找到匹配的了
            //            break;
            //        }
            //    }
            //}
            //continue;
        }

        public class ChineseFilter : IFilter
        {
            public ChineseFilter(IFilter filter) : base(filter)
            {
            }

            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                int count = userInput.Length - 1;
                string userLetters = userInput.ToLower();

                List<object> results = new List<object>();

                if (!CharUtil.IsChineseLetter(userLetters[count]))
                {
                    return results;
                }


                foreach (var obj in _unfilteredData)
                {
                    var itemLetters = GetFilterString(obj, _displayMember).ToLower();
                    if (count < itemLetters.Length)
                    {
                        char currentLetter = itemLetters[count];
                        // 如果是中文
                        if (CharUtil.IsChineseLetter(currentLetter))
                        {
                            if (userLetters[count] == currentLetter)
                            {
                                results.Add(obj);
                            }
                            continue;
                        }

                    }
                }
                return results;
            }
        }


        public class EnglishFilter : IFilter
        {
            public EnglishFilter(IFilter filter) : base(filter)
            {
            }

            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                int count = userInput.Length - 1;
                string userLetters = userInput.ToLower();

                List<object> results = new List<object>();


                foreach (var obj in _unfilteredData)
                {
                    var itemLetters = GetFilterString(obj, _displayMember).ToLower();

                    if (count < itemLetters.Length)
                    {
                        char currentLetter = itemLetters[count];

                        // 如果是英文

                        if (CharUtil.IsEnglishLetter(currentLetter))
                        {
                            if (currentLetter.ToString().ToLower()[0] == userLetters[count])
                            {
                                results.Add(obj);
                            }
                            continue;
                        }
                    }
                }
                return results;
            }

            //private bool IsEnglishLetter(char c)
            //{
            //    return (c >= 'A' && c <= 'Z' || (c >= 'a' && c <= 'z'));
            //}
        }

        public class FuzzyMatchFilter: IFilter
        {

            public FuzzyMatchFilter(IFilter filter) : base(filter)
            {
            }


            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                int count = userInput.Length - 1;
                string userLetters = userInput.ToLower();

                List<object> results = new List<object>();

                //List<string> names = new List<string>();

                //foreach (var obj in _unfilteredData)
                //{
                //    var itemLetters = GetFilterString(obj, _displayMember).ToLower();
                //    names.Add(itemLetters);
                //}

                //// 超过 % 不匹配，就过滤掉 
                //var ordered = _unfilteredData.OrderBy(x => 
                //( (double)FuzzyMatch.FindMatch(GetFilterString(x, _displayMember).ToLower(), userInput) ) / 
                //( Math.Max(GetFilterString(x, _displayMember).ToLower().Length, userInput.Length)) > RejectionPercent);

                var ordered = _unfilteredData.OrderBy(x => FuzzyMatch.FindMatch(GetFilterString(x, _displayMember).ToLower(), userInput));

                // 取最接近的几条
                results.AddRange(ordered);

                return results;
            }

            
        }

        public class DefaultFilter : IFilter
        {
            public DefaultFilter()
            {

            }
            public DefaultFilter(IFilter filter) : base(filter)
            {
            }

            protected override List<object> _Filter(List<object> _unfilteredData, string _displayMember, string userInput)
            {
                return null;
            }
        }

        public class PinyinUtil
        {
            /// <summary>
            /// 汉字转拼音首字母
            /// </summary>
            /// <param name="scrChar"></param>
            /// <returns></returns>
            public static List<string> Chinese2Pinyin(char scrChar)
            {
                try
                {
                    ChineseChar cnChar = new ChineseChar(scrChar);
                    List<string> lstStr = new List<string>(cnChar.PinyinCount);


                    for (int i = 0; i < cnChar.PinyinCount; i++)
                    {
                        // 删除声调什么的，只留ascii
                        var enletters = Regex.Replace(cnChar.Pinyins[i], @"[^a-zA-Z]", string.Empty).ToLower();
                        
                        lstStr.Add(enletters);
                    }
                    return lstStr;
                }
                catch
                {
                    return new List<string> { scrChar.ToString() };
                }
            }

            //public static List<string> Chinese2Pinyin(string rawString)
            //{
            //    Microsoft.International.Converters.PinYinConverter.
            //    List<string> s = new List<string>();
            //    try
            //    {
            //        List<List<string>> allcpinyins = new List<List<string>>();
            //        // e.g. : itemLetters = zz荣智 userInput = zzrong 
            //        foreach (var itemLetter in rawString)
            //        {
            //            if (CharUtil.IsChineseLetter(itemLetter))
            //            {
            //                List<string> cpinyins = Chinese2Pinyin(itemLetter);
            //                allcpinyins.Add(cpinyins);
            //            }

            //        }

            //        ChineseChar cnChar = new ChineseChar(scrChar);
            //        List<string> lstStr = new List<string>(cnChar.PinyinCount);
            //        for (int i = 0; i < cnChar.PinyinCount; i++)
            //        {
            //            lstStr.Add(cnChar.Pinyins[i]);
            //        }
            //        return lstStr;
            //    }
            //    catch
            //    {
            //        return new List<string> { scrChar.ToString() };
            //    }
            //}

        }

        public class CharUtil
        {
            public static bool IsChineseLetter(char c)
            {
                Match isch = Regex.Match(c.ToString(), @"[\u4e00-\u9fa5]");
                return isch.Success;
            }

            public static bool IsEnglishLetter(char c)
            {
                return (c >= 'A' && c <= 'Z' || (c >= 'a' && c <= 'z'));
            }
        }


        #endregion


        #region FuzzyMatch

        /// <summary>
        /// Contains approximate string matching
        /// </summary>
        static class LevenshteinDistance
        {
            /// <summary>
            /// Compute the distance between two strings.
            /// </summary>
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }
        }

        public class FuzzyMatch
        {
            public static int FindMatch(string a, string b)
            {
                return LevenshteinDistance.Compute(a, b);
            }
        }

        #endregion
    }


}
