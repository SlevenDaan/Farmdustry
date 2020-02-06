using Engine.InputHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics.UI
{
    public class UILayer
    {
        private int labelCount = 0;
        private UIElement<Label>[] labels = new UIElement<Label>[0];
        private Queue<int> removedLabelIds = new Queue<int>();

        private int buttonCount = 0;
        private UIElement<Button>[] buttons = new UIElement<Button>[0];
        private Queue<int> removedButtonIds = new Queue<int>();

        private int textboxCount = 0;
        private UIElement<Textbox>[] textboxes = new UIElement<Textbox>[0];
        private Queue<int> removedTextboxIds = new Queue<int>();
        private string newInputText = string.Empty;

        public SpriteFont Font { get; set; }

        /// <summary>
        /// An event that is called when a button is clicked.
        /// </summary>
        /// <remarks>The int given is the id of the button.</remarks>
        public Action<int> ButtonClicked;

        public UILayer(SpriteFont defaultFont, GameWindow window)
        {
            Font = defaultFont;
            window.TextInput += TextInput;
        }

        /// <summary>
        /// Draw all ui elements.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            DrawButtons(spriteBatch);
            DrawTextboxes(spriteBatch);
            DrawLabels(spriteBatch);
        }

        /// <summary>
        /// Update all ui elements.
        /// </summary>
        /// <param name="mouseInput">The mouseinput to check for collision and selection.</param>
        public void Update(MouseHandler mouseInput)
        {
            UpdateButtons(mouseInput);
            UpdateTextboxes(mouseInput);
        }

        #region Label
        /// <summary>
        /// Add a label to the layer.
        /// </summary>
        /// <param name="y">The y coördinate of the label.</param>
        /// <param name="x">The x coördinate of the label.</param>
        /// <param name="state">The state of the label.</param>
        /// <param name="text">The text of the label.</param>
        /// <param name="fontColor">The color of the font of the label.</param>
        /// <returns>The id of the label.</returns>
        public int AddLabel(int y, int x, UIElementState state, string text, Color fontColor)
        {
            int labelId;

            if (removedLabelIds.Count > 0)
            {
                //Get a free id from previously removed labels
                labelId = removedLabelIds.Dequeue();
            }
            else
            {
                //Make a new id
                labelId = labelCount;
                //Make the label array 1 bigger
                Array.Resize(ref labels, ++labelCount);
            }

            //Add the new label
            labels[labelId] = new UIElement<Label>(y, x, new Label(text, fontColor), state);
            return labelId;
        }

        /// <summary>
        /// Remove a label from the layer.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <returns>If the label has been removed.</returns>
        public bool RemoveLabel(int id)
        {
            if (!CheckIfLabelExists(id))
            {
                return false;
            }

            //Remove the label with the given id
            labels[id].Removed = true;
            removedLabelIds.Enqueue(id);

            return true;
        }

        /// <summary>
        /// Check if label exists.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <returns>If the label exists.</returns>
        public bool CheckIfLabelExists(int id)
        {
            return !(id >= labelCount || labels[id].Removed);
        }

        /// <summary>
        /// Set the position of a label.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <param name="y">The y coördinate of the label.</param>
        /// <param name="x">The x coördinate of the label.</param>
        /// <returns>If the label was changed.</returns>
        public bool SetLabelPosition(int id, int y, int x)
        {
            if (!CheckIfLabelExists(id))
            {
                return false;
            }

            labels[id].Position = new Point(x, y);
            return true;
        }

        /// <summary>
        /// Set the state of a label.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <param name="state">The new state of the label.</param>
        /// <returns>If the label was changed.</returns>
        public bool SetLabelState(int id, UIElementState state)
        {
            if (!CheckIfLabelExists(id))
            {
                return false;
            }

            labels[id].State = state;
            return true;
        }

        /// <summary>
        /// Set the text of a label.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <param name="text">The text of the label.</param>
        /// <returns>If the label was changed.</returns>
        public bool SetLabelText(int id, string text)
        {
            if (!CheckIfLabelExists(id))
            {
                return false;
            }

            labels[id].Element.Text = text;
            return true;
        }

        /// <summary>
        /// Set the font color of a label.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <param name="fontColor">The font color of the label.</param>
        /// <returns>If the label was changed.</returns>
        public bool SetLabelFontColor(int id, Color fontColor)
        {
            if (!CheckIfLabelExists(id))
            {
                return false;
            }

            labels[id].Element.FontColor = fontColor;
            return true;
        }

        /// <summary>
        /// Draw all visible labels.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawLabels(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < labelCount; i++)
            {
                if(labels[i].Removed || labels[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                spriteBatch.DrawString(Font, labels[i].Element.Text, labels[i].Position.ToVector2(), labels[i].Element.FontColor);
            }
        }
        #endregion

        #region Button
        /// <summary>
        /// Add a button to the layer.
        /// </summary>
        /// <param name="y">The y coördinate of the button.</param>
        /// <param name="x">The x coördinate of the button.</param>
        /// <param name="state">The state of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="fontColor">The color of the font of the button.</param>
        /// <param name="texture">The texture of the button.</param>
        /// <param name="hoverColor">The color of the button when hovered.</param>
        /// <returns>The id of the button.</returns>
        public int AddButton(int y, int x, UIElementState state, int height, int width, string text, Color fontColor, Texture2D texture, Color hoverColor)
        {
            int buttonId;

            if (removedButtonIds.Count > 0)
            {
                //Get a free id from previously removed buttons
                buttonId = removedButtonIds.Dequeue();
            }
            else
            {
                //Make a new id
                buttonId = buttonCount;
                //Make the button array 1 bigger
                Array.Resize(ref buttons, ++buttonCount);
            }

            //Add the new button
            buttons[buttonId] = new UIElement<Button>(y, x, new Button(height, width, text, fontColor, texture, hoverColor), state);
            return buttonId;
        }

        /// <summary>
        /// Remove a button from the layer.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <returns>If the button has been removed.</returns>
        public bool RemoveButton(int id)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            //Remove the button with the given id
            buttons[id].Removed = true;
            removedButtonIds.Enqueue(id);

            return true;
        }

        /// <summary>
        /// Check if button exists.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <returns>If the button exists.</returns>
        public bool CheckIfButtonExists(int id)
        {
            return !(id >= buttonCount || buttons[id].Removed);
        }

        /// <summary>
        /// Set the position of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="y">The y coördinate of the button.</param>
        /// <param name="x">The x coördinate of the button.</param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonPosition(int id, int y, int x)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Position = new Point(x, y);
            return true;
        }

        /// <summary>
        /// Set the position of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="state">The new state of the button.</param>
        /// <return>If the button was changed.</return>
        public bool SetButtonState(int id, UIElementState state)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].State = state;
            return true;
        }

        /// <summary>
        /// Set the size of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="height">The height of the button</param>
        /// <param name="width"></param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonSize(int id, int height, int width)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Element.Size = new Point(width, height);
            return true;
        }

        /// <summary>
        /// Set the text of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonText(int id, string text)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Element.Text = text;
            return true;
        }

        /// <summary>
        ///  Set the font color of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="fontColor">The font color of the button.</param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonFontColor(int id, Color fontColor)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Element.FontColor = fontColor;
            return true;
        }

        /// <summary>
        /// Set the font color of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="texture">The texture of the button.</param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonTexture(int id, Texture2D texture)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Element.Texture = texture;
            return true;
        }

        /// <summary>
        /// Set the hover color of a button.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <param name="hoverColor">The color of the button when hovered.</param>
        /// <returns>If the button was changed.</returns>
        public bool SetButtonHoverColor(int id, Color hoverColor)
        {
            if (!CheckIfButtonExists(id))
            {
                return false;
            }

            buttons[id].Element.HoverColor = hoverColor;
            return true;
        }

        /// <summary>
        /// Draw all visible buttons.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttonCount; i++)
            {
                //Check if button exists and is visible
                if (buttons[i].Removed || buttons[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                //Check button hover state to draw appropriate color
                if (buttons[i].Element.Hovered)
                {
                    spriteBatch.Draw(buttons[i].Element.Texture, new Rectangle(buttons[i].Position, buttons[i].Element.Size), buttons[i].Element.HoverColor);
                }
                else
                {
                    spriteBatch.Draw(buttons[i].Element.Texture, new Rectangle(buttons[i].Position, buttons[i].Element.Size), Color.White);
                }

                //Draw button text
                spriteBatch.DrawString(Font, buttons[i].Element.Text, buttons[i].Position.ToVector2(), buttons[i].Element.FontColor);
            }
        }

        /// <summary>
        /// Update all enabled buttons.
        /// </summary>
        /// <param name="mouseInput">The mouseinput to check for collision and selection.</param>
        private void UpdateButtons(MouseHandler mouseInput)
        {
            bool mouseClicked = !mouseInput.IsPressed(MouseButtonType.Left);
            Point mousePosition = Mouse.GetState().Position;

            for (int i = 0; i < buttonCount; i++)
            {
                //Check if button exists
                if (buttons[i].Removed)
                {
                    continue;
                }

                //Check if button is active
                if (buttons[i].State != UIElementState.Active)
                {
                    buttons[i].Element.Hovered = false;
                    continue;
                }

                //Check button hovered
                {
                    buttons[i].Element.Hovered = new Rectangle(buttons[i].Position, buttons[i].Element.Size).Contains(mousePosition);
                }

                //Check button clicked
                if (mouseClicked && buttons[i].Element.Hovered)
                {
                    ButtonClicked?.Invoke(i);
                }
            }
        }
        #endregion

        #region Textbox
        /// <summary>
        /// Add a textbox to the layer.
        /// </summary>
        /// <param name="y">The y coördinate of the textbox.</param>
        /// <param name="x">The x coördinate of the textbox.</param>
        /// <param name="state">The state of the textbox.</param>
        /// <param name="height">The height of the textbox.</param>
        /// <param name="width">The width of the textbox.</param>
        /// <param name="maxTextLength">The max length of the text of the textbox.</param>
        /// <param name="text">The text of the textbox.</param>
        /// <param name="fontColor">The color of the font of the textbox.</param>
        /// <param name="texture">The texture of the textbox.</param>
        /// <param name="selectedColor">The color of the textbox when selected.</param>
        /// <returns>The id of the textbox.</returns>
        public int AddTextbox(int y, int x, UIElementState state, int height, int width, int maxTextLength, string text, Color fontColor, Texture2D texture, Color selectedColor)
        {
            int textboxId;

            if (removedTextboxIds.Count > 0)
            {
                //Get a free id from previously removed textboxes
                textboxId = removedTextboxIds.Dequeue();
            }
            else
            {
                //Make a new id
                textboxId = textboxCount;
                //Make the textbox array 1 bigger
                Array.Resize(ref textboxes, ++textboxCount);
            }

            //Add the new textbox
            textboxes[textboxId] = new UIElement<Textbox>(y, x, new Textbox(height, width, maxTextLength, text, fontColor, texture, selectedColor), state);
            return textboxId;
        }

        /// <summary>
        /// Remove a textbox from the layer.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <returns>If the textbox has been removed.</returns>
        public bool RemoveTextbox(int id)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            //Remove the textbox with the given id
            textboxes[id].Removed = true;
            removedTextboxIds.Enqueue(id);

            return true;
        }

        /// <summary>
        /// Check if textbox exists.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <returns>If the textbox exists.</returns>
        public bool CheckIfTextboxExists(int id)
        {
            return !(id >= textboxCount || textboxes[id].Removed);
        }

        /// <summary>
        /// Set the position of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="y">The y coördinate of the textbox.</param>
        /// <param name="x">The x coördinate of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxPosition(int id, int y, int x)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Position = new Point(x, y);
            return true;
        }

        /// <summary>
        /// Set the state of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="state">The new state of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxState(int id, UIElementState state)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].State = state;
            return true;
        }

        /// <summary>
        /// Set the size of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="height">The height of the textbox.</param>
        /// <param name="width">The width of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxSize(int id, int height, int width)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Element.Size = new Point(width, height);
            return true;
        }

        /// <summary>
        /// Set the maximum length of the text of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="maxTextLength">The max length of the text of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxMaxTextLength(int id, int maxTextLength)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Element.MaxTextLength = maxTextLength;
            return true;
        }

        /// <summary>
        /// Set the text of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="text">The text of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxText(int id, string text)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            //Remove access text
            if (text.Length > textboxes[id].Element.MaxTextLength)
            {
                text = text.Substring(0, textboxes[id].Element.MaxTextLength);
            }
            textboxes[id].Element.Text = text;
            
            return true;
        }

        /// <summary>
        /// Set the font color of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="fontColor">The font color of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxFontColor(int id, Color fontColor)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Element.FontColor = fontColor;
            return true;
        }

        /// <summary>
        /// Set the texture of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="texture">The texture of the textbox.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxTexture(int id, Texture2D texture)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Element.Texture = texture;
            return true;
        }

        /// <summary>
        /// Set the selected color of a textbox.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <param name="selectedColor">The color of the textbox when selected.</param>
        /// <returns>If the textbox was changed.</returns>
        public bool SetTextboxSelectedColor(int id, Color selectedColor)
        {
            if (!CheckIfTextboxExists(id))
            {
                return false;
            }

            textboxes[id].Element.SelectedColor = selectedColor;
            return true;
        }

        /// <summary>
        /// Draw all visible buttons.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawTextboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < textboxCount; i++)
            {
                //Check if textbox exists and is visible
                if (textboxes[i].Removed || textboxes[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                //Check textbox selected state to draw appropriate color
                if (textboxes[i].Element.Selected)
                {
                    spriteBatch.Draw(textboxes[i].Element.Texture, new Rectangle(textboxes[i].Position, textboxes[i].Element.Size), textboxes[i].Element.SelectedColor);
                }
                else
                {
                    spriteBatch.Draw(textboxes[i].Element.Texture, new Rectangle(textboxes[i].Position, textboxes[i].Element.Size), Color.White);
                }

                //Draw textbox text
                spriteBatch.DrawString(Font, textboxes[i].Element.Text, textboxes[i].Position.ToVector2(), textboxes[i].Element.FontColor);
            }
        }

        /// <summary>
        /// Update all enabled textboxes.
        /// </summary>
        /// <param name="mouseInput">The mouseinput to check for collision and selection.</param>
        private void UpdateTextboxes(MouseHandler mouseInput)
        {
            bool mouseClicked = mouseInput.IsPressed(MouseButtonType.Left);
            bool textTyped = newInputText != string.Empty;
            Point mousePosition = mouseInput.Position;

            for (int i = 0; i < textboxCount; i++)
            {
                //Check if textbox exists and is active
                if (textboxes[i].Removed || textboxes[i].State != UIElementState.Active)
                {
                    continue;
                }

                //Add typed text to textbox
                if (textTyped && textboxes[i].Element.Selected)
                {
                    textboxes[i].Element.Text += newInputText;
                    //Remove access text
                    if (textboxes[i].Element.Text.Length > textboxes[i].Element.MaxTextLength)
                    {
                        textboxes[i].Element.Text = textboxes[i].Element.Text.Substring(0, textboxes[i].Element.MaxTextLength);
                    }
                }

                //Check selected state
                if (mouseClicked)
                {
                    textboxes[i].Element.Selected = new Rectangle(textboxes[i].Position, textboxes[i].Element.Size).Contains(mousePosition);
                }
            }

            newInputText = string.Empty;
        }
        #endregion

        private void TextInput(object sender, TextInputEventArgs e)
        {
            newInputText += e.Character;
        }
    }
}
