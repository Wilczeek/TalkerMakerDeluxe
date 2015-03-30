﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeContainer;
using System.IO;
using System.Windows.Markup;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using FontAwesome.WPF;
using MahApps.Metro.Controls;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Reflection;
using MahApps.Metro;
using System.Collections;
using Memento;



namespace TalkerMakerDeluxe
{
    public partial class MainWindow : MetroWindow
	{

        TalkerMakerProject projie;
        List<int> handledNodes = new List<int>();
        List<DialogHolder> IDs = new List<DialogHolder>();
        string currentNode = "";
        int loadedConversation = -1;
        public struct DialogHolder
        {
            public int ID;
            public List<int> ChildNodes;
        }
        Mementor mementorTalker = new Mementor();



        public MainWindow()
		{
			InitializeComponent();

            this.Icon = ImageAwesome.CreateImageSource(FontAwesomeIcon.CommentsO, (Brush)Application.Current.FindResource("HighlightBrush"));

            RoutedCommand saveProject = new RoutedCommand();
            saveProject.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveProject, SaveHandler));

            
            projie = XMLHandler.LoadXML("C:\\Users\\Randall\\Downloads\\Example_XML_Export.xml");
            tabBlank.IsSelected = true;
            lstCharacters.ItemsSource = AddActors(projie);
            lstConversations.ItemsSource = AddConversations(projie);

            uiScaleSlider.MouseDoubleClick +=
            new MouseButtonEventHandler(RestoreScalingFactor);

            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (Stream s = _assembly.GetManifestResourceStream("TalkerMakerDeluxe.Lua.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    editConditions.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            editScript.SyntaxHighlighting = editConditions.SyntaxHighlighting;

		}

        void RestoreScalingFactor(object sender, MouseButtonEventArgs args)
        {
            ((Slider)sender).Value = 1.0;
        }

		private void tcMain_Click(object sender, RoutedEventArgs e)
		{
            /*string ham = e.OriginalSource.GetType().ToString();
            if(ham == "System.Windows.Controls.Button")
            { 
			    Button btn = e.OriginalSource as Button;
                Grid grid = btn.Parent as Grid;
                Border border = grid.Parent as Border;
                NodeControl ndctl = border.Parent as NodeControl;
                if (btn != null)
                {
                    TreeNode tn = (TreeNode)(ndctl.Parent);
                    tn.Collapsed = !tn.Collapsed;
                    if (ndctl.faMin.Icon == FontAwesomeIcon.AngleDoubleUp)
                    {
                        ndctl.faMin.Icon = FontAwesomeIcon.AngleDoubleDown;
                    }
                    else
                    {
                        ndctl.faMin.Icon = FontAwesomeIcon.AngleDoubleUp;
                    }
                }
            }*/
		}

        public void CollapseNode(string parentNode)
        {
            TreeNode tn = tcMain.FindName(parentNode.Remove(0, 1)) as TreeNode;
            NodeControl ndctl = tn.Content as NodeControl;
            tn.Collapsed = !tn.Collapsed;
            if (ndctl.faMin.Icon == FontAwesomeIcon.AngleDoubleUp)
            {
                ndctl.faMin.Icon = FontAwesomeIcon.AngleDoubleDown;
            }
            else
            {
                ndctl.faMin.Icon = FontAwesomeIcon.AngleDoubleUp;
            }
        }

        public void AddNode(string parentNode)
        {
            if (loadedConversation != -1)
            {
                
                TreeNode nodeTree = tcMain.FindName(parentNode.Remove(0, 1)) as TreeNode;
                NodeControl ndctl = nodeTree.Content as NodeControl;

                DialogEntry newDialogue = new DialogEntry();
                Field newDialogueField_1 = new Field();
                Field newDialogueField_2 = new Field();
                Field newDialogueField_3 = new Field();
                Field newDialogueField_4 = new Field();
                Field newDialogueField_5 = new Field();
                Link newDialogueLink = new Link();
                NodeControl newDialogueNode = new NodeControl();
                CharacterItem firstActor = lstCharacters.Items[0] as CharacterItem;
                int parentID = (int)ndctl.lblID.Content;
                int newNodeID = projie.Assets.Conversations[loadedConversation].DialogEntries.OrderByDescending(p => p.ID).First().ID + 1;

                //Create Dialogue Item in Project
                newDialogue.ID = newNodeID;
                newDialogueField_1.Type = "Text";
                newDialogueField_1.Title = "Title";
                newDialogueField_1.Value = "New Dialogue";
                newDialogue.Fields.Add(newDialogueField_1);
                newDialogueField_2.Type = "Actor";
                newDialogueField_2.Title = "Actor";
                newDialogueField_2.Value = firstActor.lblActorID.Content.ToString();
                newDialogue.Fields.Add(newDialogueField_2);
                newDialogueField_3.Type = "Actor";
                newDialogueField_3.Title = "Conversant";
                newDialogueField_3.Value = firstActor.lblActorID.Content.ToString();
                newDialogue.Fields.Add(newDialogueField_3);
                newDialogueField_4.Type = "Text";
                newDialogueField_4.Title = "Menu Text";
                newDialogueField_4.Value = "";
                newDialogue.Fields.Add(newDialogueField_4);
                newDialogueField_5.Type = "Text";
                newDialogueField_5.Title = "Dialogue Text";
                newDialogueField_5.Value = "";
                newDialogue.Fields.Add(newDialogueField_5);

                //Add to conversation
                projie.Assets.Conversations[loadedConversation].DialogEntries.Add(newDialogue);
                //Set link to parent.
                newDialogueLink.DestinationConvoID = loadedConversation;
                newDialogueLink.OriginConvoID = loadedConversation;
                newDialogueLink.OriginDialogID = parentID;
                newDialogueLink.DestinationDialogID = newNodeID;
                projie.Assets.Conversations[loadedConversation].DialogEntries.Where(p => p.ID == parentID).First().OutgoingLinks.Add(newDialogueLink);

                //Setup for Physical Node
                newDialogueNode.Name = "_node_" + newNodeID;
                newDialogueNode.lblID.Content = newNodeID;
                newDialogueNode.lblDialogueName.Content = "New Dialogue";
                newDialogueNode.lblConversantID.Content = firstActor.lblActorID.Content;
                newDialogueNode.lblActorID.Content = firstActor.lblActorID.Content;
                newDialogueNode.lblActor.Content = firstActor.lblActorName.Content;
                newDialogueNode.lblConversant.Content = firstActor.lblActorName.Content;


                //Add to tree.
                tcMain.AddNode(newDialogueNode, "node_" + newNodeID, "node_" + parentID);
            }
        }

        public void SelectNode(string newNode)
        {
            if (currentNode != "" && newNode != currentNode)
                {
                    //Color newNode
                    TreeNode nodeTree = tcMain.FindName(newNode.Remove(0, 1)) as TreeNode;
                    NodeControl node = nodeTree.Content as NodeControl;
                    node.border.BorderBrush = (Brush)Application.Current.FindResource("BlackBrush");
                    



                    //Remove color from currentNode
                    nodeTree = tcMain.FindName(currentNode.Remove(0, 1)) as TreeNode;
                    node = nodeTree.Content as NodeControl;
                    node.border.BorderBrush = (Brush)Application.Current.FindResource("HighlightBrush");
                    currentNode = newNode;

                    tabDialogue.IsSelected = true;
                }
                else if (newNode != currentNode)
                {
                    TreeNode nodeTree = tcMain.FindName(newNode.Remove(0, 1)) as TreeNode;
                    NodeControl node = nodeTree.Content as NodeControl;

                    tcMain.ToString();
                    node.border.BorderBrush = (Brush)Application.Current.FindResource("BlackBrush") as Brush;
                    currentNode = newNode;

                    tabDialogue.IsSelected = true;
                }
            if(newNode == null)
            {

                TreeNode nodeTree = tcMain.FindName(currentNode.Remove(0, 1)) as TreeNode;
                NodeControl node = nodeTree.Content as NodeControl;
                node.border.BorderBrush = (Brush)Application.Current.FindResource("HighlightBrush");
            }
            
        }

        private void DrawConversationTree(DialogHolder dh)
        {
            
            if (!handledNodes.Contains(dh.ID))
            {
                handledNodes.Add(dh.ID);
                int parentNode = -1;
                DialogEntry de = projie.Assets.Conversations[loadedConversation].DialogEntries.First(d => d.ID == dh.ID);
                NodeControl ndctl = new NodeControl();
                ndctl.lblID.Content = de.ID;
                ndctl.Name = "_node_" + de.ID;
                ndctl.lblUserScript.Content = de.UserScript;
                ndctl.lblConditionsString.Content = de.ConditionsString;
                Console.WriteLine("Setting Bindings...");
                foreach (Field field in de.Fields)
                {
                    switch (field.Title)
                    {
                        case "Title":
                            ndctl.lblDialogueName.Content = field.Value;
                            break;
                        case "Actor":
                            ndctl.lblActorID.Content = field.Value;
                            CharacterItem chara = lstCharacters.Items[Convert.ToInt16(field.Value) - 1] as CharacterItem;
                            ndctl.lblActor.Content = chara.lblActorName.Content;
                            break;
                        case "Conversant":
                            ndctl.lblConversantID.Content = field.Value;
                            chara = lstCharacters.Items[Convert.ToInt16(field.Value) - 1] as CharacterItem;
                            ndctl.lblConversant.Content = chara.lblActorName.Content;
                            break;
                        case "Menu Text":
                            ndctl.lblMenuText.Content = field.Value;
                            break;
                        case "Dialogue Text":
                            ndctl.txtDialogue.Text = field.Value;
                            break;
                    }
                }
                foreach(DialogHolder dhParent in IDs)
                {
                    if (dhParent.ChildNodes.Contains(dh.ID))
                        parentNode = dhParent.ID;
                }
                if (parentNode == -1)
                {
                    tcMain.AddRoot(ndctl, "node_" + dh.ID);
                    //tcMain.RegisterName("_node_" + dial.ID, ndctl);
                    Console.WriteLine("Writing root: " + dh.ID);
                }
                else
                {
                    tcMain.AddNode(ndctl, "node_" + dh.ID, "node_" + parentNode);
                    //tcMain.RegisterName("_node_" + dial.ID, ndctl);
                    Console.WriteLine("Writing node: " + dh.ID);
                }
            }
        }

        #region List Fill Functions
        private List<ConversationItem> AddConversations(TalkerMakerProject project)
        {
            List<ConversationItem> conversations = new List<ConversationItem>();
            foreach(Conversation conversation in project.Assets.Conversations)
            {
                ConversationItem conv = new ConversationItem();
                conv.lblConvID.Content = conversation.ID;
                foreach(Field field in conversation.Fields)
                {
                    switch(field.Title)
                    {
                        case "Title":
                            conv.lblConvTitle.Content = field.Value;
                            break;
                        case "Actor":
                            conv.lblConvActorID.Content = field.Value;
                            CharacterItem chara = lstCharacters.Items[Convert.ToInt16(field.Value)-1] as CharacterItem;
                            conv.lblConvActor.Content = chara.lblActorName.Content;
                            break;
                        case "Conversant":
                            conv.lblConvConversantID.Content = field.Value;
                            chara = lstCharacters.Items[Convert.ToInt16(field.Value)-1] as CharacterItem;
                            conv.lblConvConversant.Content = chara.lblActorName.Content;
                            break;
                        case "Description":
                            conv.lblConvDescription.Content = field.Value;
                            break;
                        case "Scene":
                            conv.lblconvScene.Content = field.Value;
                            break;
                    }
                }

                conversations.Add(conv);
            }

            return conversations;
        }

        private List<CharacterItem> AddActors(TalkerMakerProject project)
        {
            List<CharacterItem> actors = new List<CharacterItem>();
            foreach (Actor actor in project.Assets.Actors)
            {
                CharacterItem chara = new CharacterItem();
                chara.lblActorID.Content = actor.ID;
                chara.lblActorDescription.Content = "";
                chara.lblActorAge.Content = "";
                chara.lblActorGender.Content = "";
                chara.lblActorPicture.Content = "";
                chara.Name = "actor_" + actor.ID;
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "Name":
                            chara.lblActorName.Content = field.Value;
                            break;
                        case "Age":
                            chara.lblActorAge.Content = field.Value;
                            break;
                        case "Gender":
                            chara.lblActorGender.Content = field.Value;
                            break;
                        case "IsPlayer":
                            chara.lblActorIsPlayer.Content = field.Value;
                            break;
                        case "Description":
                            chara.lblActorDescription.Content = field.Value;
                            break;
                        case "Pictures":
                            if(IsBase64(field.Value))
                            {
                                chara.imgActorImage.Source = Base64ToImage(field.Value);
                                chara.lblActorPicture.Content = field.Value;
                            }
                            break;
                    }
                }
                actors.Add(chara);
            }

            return actors;
        }
        #endregion

        #region Custom PropertyGrid Editors
        public class FileChooserEditor : ITypeEditor
        {
            TextBox tb;

            public FrameworkElement ResolveEditor(PropertyItem propertyItem)
            {
                DockPanel dp = new DockPanel();
                dp.LastChildFill = true;
                Button bt = new Button();
                bt.Content = "...";
                bt.Click += new RoutedEventHandler(bt_Click);
                DockPanel.SetDock(bt, Dock.Right);
                dp.Children.Add(bt);
                tb = new TextBox();
                tb.Text = "xyz";
                dp.Children.Add(tb);

                //create the binding from the bound property item to the editor
                var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
                _binding.Source = propertyItem;
                _binding.ValidatesOnExceptions = true;
                _binding.ValidatesOnDataErrors = true;
                _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
                BindingOperations.SetBinding(tb, TextBox.TextProperty, _binding);
                return dp;
            }

            void bt_Click(Object sender, RoutedEventArgs e)
            {
                Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
                //setOpenFileDialog(openFile);
                if (openFile.ShowDialog() == true)
                {
                    tb.Text = openFile.FileName;
                    BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                    be.UpdateSource();
                }
            }

        }

        public class MultiLineTextEditor : ITypeEditor
        {
            public FrameworkElement ResolveEditor(PropertyItem propertyItem)
            {
                TextBox editor = new TextBox();
                editor.TextWrapping = TextWrapping.Wrap;
                editor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                Binding binding = new Binding("Value"); //bind to the Value property of the PropertyItem instance
                binding.Source = propertyItem;
                binding.Mode = propertyItem.IsReadOnly ? BindingMode.TwoWay : BindingMode.OneWay;
                BindingOperations.SetBinding(editor, TextBox.TextProperty, binding);
                return editor;
            }
        }
        #endregion


        #region Front-End Functions
        private void SaveHandler(object obSender, ExecutedRoutedEventArgs e)
        {
            // Do the Save All thing here.
            Console.WriteLine("Saving...");
            XMLHandler.SaveXML(projie, "C:\\Users\\Randall\\Downloads\\Example_XML_Export.xml");
            Console.WriteLine("Save finished.");
        }

        private void lstCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            txtActorID.Text = chara.lblActorID.Content.ToString();
            txtActorName.Text = chara.lblActorName.Content.ToString();
            txtActorAge.Value = chara.lblActorAge.Content != "" ? Convert.ToInt16(chara.lblActorAge.Content) : 0;
            txtActorGender.Text = chara.lblActorGender.Content.ToString();
            txtActorDescription.Text = chara.lblActorDescription.Content.ToString();
            txtActorPicture.Text = chara.lblActorPicture.Content.ToString();
            imgActorPicture.Source = chara.imgActorImage.Source;
            chkActorPlayer.IsChecked = Convert.ToBoolean(chara.lblActorIsPlayer.Content);
            tabCharacter.IsSelected = true;
            
        }

        private void lstConversations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConversationItem conv = lstConversations.SelectedItem as ConversationItem;

            tabConversation.IsSelected = true;
        }

        private void lstConversations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currentNode = "";
            tcMain.Clear();
            //DrawConversationTree(projie.Assets.Conversations[lstConversations.SelectedIndex], projie.Assets.Conversations[lstConversations.SelectedIndex].DialogEntries[0]);
            IDs.Clear();
            loadedConversation = lstConversations.SelectedIndex;
            foreach (DialogEntry d in projie.Assets.Conversations[loadedConversation].DialogEntries)
            {
                List<int> childs = new List<int>();
                foreach(Link link in d.OutgoingLinks)
                {
                    if(link.DestinationConvoID == link.OriginConvoID && link.IsConnector == false)
                        childs.Add(link.DestinationDialogID);
                }
                DialogHolder dh = new DialogHolder();
                dh.ID = d.ID;
                dh.ChildNodes = childs;
                IDs.Add(dh);
            }
            foreach(DialogHolder dh in IDs)
            {
                DrawConversationTree(dh);
            }
            tcMain.Children.OfType<TreeNode>().First(node => node.Name == "node_0").BringIntoView();
            //scrlTree.ScrollToHorizontalOffset(scrlTree.HorizontalOffset + 200);
            handledNodes.Clear();
        }

        private void cmbZoomPercent_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int n;
                bool isNumeric = int.TryParse(cmbZoomPercent.Text.Replace("%", ""), out n);
                if (isNumeric)
                {
                    cmbZoomPercent.Text = cmbZoomPercent.Text.Replace("%", "") + "%";
                }
                else
                {
                    cmbZoomPercent.Text = "100%";
                }
                uiScaleSlider.Value = Convert.ToInt16(cmbZoomPercent.Text.Replace("%", "")) / 100;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cmbZoomPercent_LostFocus(object sender, RoutedEventArgs e)
        {
            int n;
            bool isNumeric = int.TryParse(cmbZoomPercent.Text.Replace("%", ""), out n);
            if(isNumeric)
            {
                cmbZoomPercent.Text = cmbZoomPercent.Text.Replace("%", "") + "%";
            }
            else
            {
                cmbZoomPercent.Text = "100%";
            }
            uiScaleSlider.Value = Convert.ToInt16(cmbZoomPercent.Text.Replace("%", "")) / 100;
        }

        private void cmbZoomPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            uiScaleSlider.Value = Convert.ToInt16(cmbZoomPercent.Text.Replace("%", "")) / 100;
        }
        #endregion


        #region Actor Functions
        private void txtActorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            if (txtActorName.Text != "" && chara.lblActorName.Content != txtActorName.Text)
            {
                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "Name":
                            field.Value = txtActorName.Text;
                            break;
                    }
                }
                chara.lblActorName.Content = txtActorName.Text;
            }
        }



        private void txtActorGender_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            if (txtActorGender.Text != "" && chara.lblActorGender.Content != txtActorGender.Text)
            {
                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                int containsGender = 0;
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "Gender":
                            field.Value = txtActorGender.Text;
                            containsGender = 1;
                            break;
                    }
                }
                if(containsGender == 0)
                {
                    Field addField = new Field();
                    addField.Title = "Gender";
                    addField.Value = txtActorGender.Text;
                    actor.Fields.Add(addField);
                }
                chara.lblActorGender.Content = txtActorGender.Text;
            }
        }

        private void chkActorPlayer_Checked(object sender, RoutedEventArgs e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            if (Convert.ToBoolean(chara.lblActorIsPlayer.Content) != chkActorPlayer.IsChecked)
            {
                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "IsPlayer":
                            field.Value = chkActorPlayer.IsChecked.ToString();
                            break;
                    }
                }
                chara.lblActorIsPlayer.Content = chkActorPlayer.IsChecked.ToString();
            }
        }

        private void txtActorAge_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            if (txtActorAge.Value.ToString() != "" && chara.lblActorAge.Content != txtActorAge.Value.ToString())
            {
                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                int containsAge = 0;
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "Age":
                            field.Value = txtActorAge.Value.ToString();
                            containsAge = 1;
                            break;
                    }
                }
                if (containsAge == 0)
                {
                    Field addField = new Field();
                    addField.Title = "Age";
                    addField.Value = txtActorAge.Value.ToString();
                    actor.Fields.Add(addField);
                }
                chara.lblActorAge.Content = txtActorAge.Value.ToString();
            }
        }

        private void txtActorDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
            if (txtActorDescription.Text != "" && chara.lblActorDescription.Content != txtActorDescription.Text)
            {
                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                int containsDescription = 0;
                foreach (Field field in actor.Fields)
                {
                    switch (field.Title)
                    {
                        case "Description":
                            field.Value = txtActorDescription.Text;
                            containsDescription = 1;
                            break;
                    }
                }
                if (containsDescription == 0)
                {
                    Field addField = new Field();
                    addField.Title = "Description";
                    addField.Value = txtActorDescription.Text;
                    actor.Fields.Add(addField);
                }
                chara.lblActorDescription.Content = txtActorDescription.Text;
            }
        }

        private void btnPicturePicker_Click(object sender, RoutedEventArgs e)
        {
            if(lstCharacters.SelectedItem != null)
            {
                CharacterItem chara = lstCharacters.SelectedItem as CharacterItem;
                
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Image Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //if(txtActorPicture.Text != "")
                    //    openFileDialog.InitialDirectory = txtActorPicture.Text;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        

                            string actorImageString = ImageToBase64(openFileDialog.FileName);
                            txtActorPicture.Text = actorImageString;

                            BitmapImage actorImage = Base64ToImage(txtActorPicture.Text);
                            imgActorPicture.Source = actorImage;

                            if (txtActorPicture.Text != "" && chara.lblActorPicture.Content != txtActorPicture.Text)
                            {
                                chara.lblActorPicture.Content = actorImageString;
                                chara.imgActorImage.Source = actorImage;

                                Actor actor = projie.Assets.Actors[Convert.ToInt16(chara.lblActorID.Content) - 1];
                                int containsDescription = 0;
                                foreach (Field field in actor.Fields)
                                {
                                    switch (field.Title)
                                    {
                                        case "Pictures":
                                            field.Value = actorImageString;
                                            containsDescription = 1;
                                            break;
                                    }
                                }
                                if (containsDescription == 0)
                                {
                                    Field addField = new Field();
                                    addField.Title = "Pictures";
                                    addField.Value = actorImageString;
                                    actor.Fields.Add(addField);
                                }
                            }
                       }
                
            }
        }

        public BitmapImage Base64ToImage(string base64String)
        {
            byte[] binaryData = Convert.FromBase64String(base64String);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = 64;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            return bi;
        }

        public string ImageToBase64(string imgString)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imgString);
            bi.DecodePixelWidth = 64;
            bi.EndInit();
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));
            encoder.Save(memStream);
            string binaryData = Convert.ToBase64String(memStream.GetBuffer());
            return binaryData;
        }

        public bool IsBase64(string base64String)
        {
            if (base64String == null || base64String.Length == 0 || base64String.Length % 4 != 0
               || base64String.Contains(' ') || base64String.Contains('\t') || base64String.Contains('\r') || base64String.Contains('\n'))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                
            }
            return false;
        }
        #endregion

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Saving...");
            XMLHandler.SaveXML(projie, "C:\\Users\\Randall\\Downloads\\Example_XML_Export.xml");
            Console.WriteLine("Save finished.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mementorTalker.CanUndo)
                mementorTalker.Undo();
        }

        


       
    }


    
}
