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

        private int buttonCount = 0;
        private UIElement<Button>[] buttons = new UIElement<Button>[0];

        private int textboxCount = 0;
        private UIElement<Textbox>[] textboxes = new UIElement<Textbox>[0];
        private string newInputText = string.Empty;

        public SpriteFont Font { get; set; }

        /// <summary>
        /// An event that is alled when a button is clicked.
        /// </summary>
        /// <remarks>The int given is the id of the button.</remarks>
        public Action<int> ButtonClicked;

        public UILayer(GameWindow window)
        {
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
        public void Update(MouseInputHandler mouseInput)
        {
            UpdateButtons(mouseInput);
            UpdateTextboxes(mouseInput);
        }

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
            //Make the label array 1 bigger
            UIElement<Label>[] oldLabels = labels;
            labels = new UIElement<Label>[labelCount + 1];
            oldLabels.CopyTo(labels, 0);

            //Add the new label
            labels[labelCount] = new UIElement<Label>(y, x, new Label(text, fontColor), state);
            return buttons[buttonCount++].Id;
        }
        /// <summary>
        /// Remove a label from the layer.
        /// </summary>
        /// <param name="id">The id of the label.</param>
        /// <returns>If the label has been removed.</returns>
        public bool RemoveLabel(int id)
        {
            //Remove the label with the given id
            labels = labels.Where(label => label.Id != id).ToArray();

            //Check if a label got removed
            if (labels.Length < labelCount)
            {
                labelCount--;
                return true;
            }

            return false;
        }
        /// <summary>
        /// Draw all visible labels.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawLabels(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < labelCount; i++)
            {
                if(labels[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                spriteBatch.DrawString(Font, labels[i].Element.Text, labels[i].Position.ToVector2(), labels[i].Element.FontColor);
            }
        }

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
            //Make the button array 1 bigger
            UIElement<Button>[] oldButtons = buttons;
            buttons = new UIElement<Button>[buttonCount + 1];
            oldButtons.CopyTo(buttons, 0);

            //Add the new button
            buttons[buttonCount] = new UIElement<Button>(y, x, new Button(height, width, text, fontColor, texture, hoverColor), state);
            return buttons[buttonCount++].Id;
        }
        /// <summary>
        /// Remove a button from the layer.
        /// </summary>
        /// <param name="id">The id of the button.</param>
        /// <returns>If the button has been removed.</returns>
        public bool RemoveButton(int id)
        {
            //Remove the button with the given id
            buttons = buttons.Where(button => button.Id != id).ToArray();

            //Check if a button got removed
            if (buttons.Length < buttonCount)
            {
                buttonCount--;
                return true;
            }

            return false;
        }
        /// <summary>
        /// Draw all visible buttons.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttonCount; i++)
            {
                if (buttons[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                if (buttons[i].Element.Hovered)
                {
                    spriteBatch.Draw(buttons[i].Element.Texture, new Rectangle(buttons[i].Position, buttons[i].Element.Size), buttons[i].Element.HoverColor);
                }
                else
                {
                    spriteBatch.Draw(buttons[i].Element.Texture, new Rectangle(buttons[i].Position, buttons[i].Element.Size), Color.White);
                }
                spriteBatch.DrawString(Font, buttons[i].Element.Text, buttons[i].Position.ToVector2(), buttons[i].Element.FontColor);
            }
        }
        /// <summary>
        /// Update all enabled buttons.
        /// </summary>
        /// <param name="mouseInput">The mouseinput to check for collision and selection.</param>
        private void UpdateButtons(MouseInputHandler mouseInput)
        {
            bool mouseClicked = !mouseInput.IsPressed(MouseButtonType.Left);
            Point mousePosition = Mouse.GetState().Position;

            for (int i = 0; i < buttonCount; i++)
            {
                if (buttons[i].State != UIElementState.Active)
                {
                    buttons[i].Element.Hovered = false;
                    continue;
                }

                buttons[i].Element.Hovered = new Rectangle(buttons[i].Position, buttons[i].Element.Size).Contains(mousePosition);

                if (mouseClicked && buttons[i].Element.Hovered)
                {
                    ButtonClicked?.Invoke(buttons[i].Id);
                }
            }
        }

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
        /// <param name="selecedColor">The color of the textbox when selected.</param>
        /// <returns>The id of the textbox.</returns>
        public int AddTextbox(int y, int x, UIElementState state, int height, int width, int maxTextLength, string text, Color fontColor, Texture2D texture, Color selecedColor)
        {
            //Make the textbox array 1 bigger
            UIElement<Textbox>[] oldTextboxes = textboxes;
            textboxes = new UIElement<Textbox>[textboxCount + 1];
            oldTextboxes.CopyTo(textboxes, 0);

            //Add the new textbox
            textboxes[textboxCount] = new UIElement<Textbox>(y, x, new Textbox(height, width, maxTextLength, text, fontColor, texture, selecedColor), state);
            return textboxes[textboxCount++].Id;
        }
        /// <summary>
        /// Remove a textbox from the layer.
        /// </summary>
        /// <param name="id">The id of the textbox.</param>
        /// <returns>If the textbox has been removed.</returns>
        public bool RemoveTextbox(int id)
        {
            //Remove the textbox with the given id
            textboxes = textboxes.Where(textbox => textbox.Id != id).ToArray();

            //Check if a textbox got removed
            if (textboxes.Length < textboxCount)
            {
                textboxCount--;
                return true;
            }

            return false;
        }
        /// <summary>
        /// Draw all visible buttons.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing.</param>
        private void DrawTextboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < textboxCount; i++)
            {
                if (textboxes[i].State == UIElementState.Hidden)
                {
                    continue;
                }

                if (textboxes[i].Element.Selected)
                {
                    spriteBatch.Draw(textboxes[i].Element.Texture, new Rectangle(textboxes[i].Position, textboxes[i].Element.Size), textboxes[i].Element.SelectedColor);
                }
                else
                {
                    spriteBatch.Draw(textboxes[i].Element.Texture, new Rectangle(textboxes[i].Position, textboxes[i].Element.Size), Color.White);
                }
                spriteBatch.DrawString(Font, textboxes[i].Element.Text, textboxes[i].Position.ToVector2(), textboxes[i].Element.FontColor);
            }
        }
        /// <summary>
        /// Update all enabled textboxes.
        /// </summary>
        /// <param name="mouseInput">The mouseinput to check for collision and selection.</param>
        private void UpdateTextboxes(MouseInputHandler mouseInput)
        {
            bool mouseClicked = !mouseInput.IsPressed(MouseButtonType.Left);
            bool textTyped = newInputText != string.Empty;
            Point mousePosition = mouseInput.Position;

            for (int i = 0; i < textboxCount; i++)
            {
                if (textboxes[i].State != UIElementState.Active)
                {
                    continue;
                }

                if (textTyped)
                {
                    textboxes[i].Element.Text += newInputText;
                    //Remove access text
                    if(textboxes[i].Element.Text.Length> textboxes[i].Element.MaxTextLength)
                    {
                        textboxes[i].Element.Text = textboxes[i].Element.Text.Substring(0, textboxes[i].Element.MaxTextLength);
                    }
                }

                if (mouseClicked)
                {
                    textboxes[i].Element.Selected = new Rectangle(textboxes[i].Position, textboxes[i].Element.Size).Contains(mousePosition);
                }
            }

            newInputText = string.Empty;
        }
        
        private void TextInput(object sender, TextInputEventArgs e)
        {
            newInputText += e.Character;
        }
    }
}
