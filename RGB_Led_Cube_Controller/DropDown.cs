using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RGB_Led_Cube_Controller
{

    public class DropDown : DrawableGameComponent
    {
        public delegate void EventHandler_ID(int ID);
        public event EventHandler_ID event_selectionchange;
        private Vector2 pos, size_item, selected_title_size;
        private SpriteFont font_selected, font_items;
        private Color col_selected, col_items, col_items_hover, col_edge, col_selected_text, col_items_text;
        private int itemnum, currentselection = 0, state = 0;
        private event EventHandler selection_change;
        private List<string> item_titles;
        private List<Vector2> title_sizes;

        public DropDown(SpriteFont fonts, Vector2 pos, Vector2 size_item, float maxheight) : base(Game1.maingame)
        {
            font_selected = font_items = fonts;
            col_selected = Color.LightGreen;
            col_selected_text = col_items_text = new Color(new Vector3(0.4f));
            col_items = Color.WhiteSmoke;
            col_items_hover = Color.Orange;
            col_edge = Color.Black;
            this.pos = pos;
            this.size_item = size_item;
            item_titles = new List<string>();
            title_sizes = new List<Vector2>();
            itemnum = 0;
        }

        public void AddItem(string text)
        {
            item_titles.Add(text);
            title_sizes.Add(font_items.MeasureString(text));
            if (itemnum == 0)
                selected_title_size = font_selected.MeasureString(text);
            itemnum++;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 mousepos = Game1.mousestate.Position.ToVector2();
            if (state == 0 && itemnum > 0 && mousepos.X >= pos.X && mousepos.X < pos.X + size_item.X && mousepos.Y >= pos.Y && mousepos.Y < pos.Y + size_item.Y && Game1.mousestate.LeftButton == ButtonState.Pressed && Game1.oldmousestate.LeftButton == ButtonState.Released)
                state = 1;
            if (state == 1 && !(mousepos.X >= pos.X && mousepos.X < pos.X + size_item.X && mousepos.Y >= pos.Y && mousepos.Y < pos.Y + size_item.Y * (itemnum + 1) + 2))
                state = 0;
            if (state == 1 && mousepos.X >= pos.X && mousepos.X < pos.X + size_item.X && mousepos.Y >= pos.Y + size_item.Y + 2 && mousepos.Y < pos.Y + size_item.Y * (itemnum + 1) + 2 && Game1.mousestate.LeftButton == ButtonState.Pressed)
            {
                int id = (int)((mousepos.Y - pos.Y - size_item.Y - 2) / size_item.Y);
                if (id != currentselection && id < itemnum)
                {
                    currentselection = id;
                    selected_title_size = font_selected.MeasureString(item_titles[id]);
                    event_selectionchange(id);
                }
            }
            //base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 mousepos = Game1.mousestate.Position.ToVector2();
            Game1.spriteBatch.Begin();
            Game1.DrawRectangle_Filled(pos, size_item, col_selected);
            if (itemnum > 0)
            {

                Game1.spriteBatch.DrawString(font_selected, item_titles[currentselection], pos + size_item / 2 - selected_title_size / 2, col_selected_text);
                if (state == 1)
                {
                    int hover_id = (int)((mousepos.Y - pos.Y - size_item.Y - 2) / size_item.Y);
                    for (int i = 0; i < itemnum; ++i)
                    {
                        if (hover_id == i)
                            Game1.DrawRectangle_Filled(pos + new Vector2(0, size_item.Y * (i + 1) + 2), size_item, col_items_hover);
                        else
                            Game1.DrawRectangle_Filled(pos + new Vector2(0, size_item.Y * (i + 1) + 2), size_item, col_items);
                        Game1.DrawLine(Game1.spriteBatch, pos + new Vector2(0, (i + 1) * size_item.Y + 2), pos + new Vector2(size_item.X, (i + 1) * size_item.Y + 2), Color.LightGray);
                        Game1.spriteBatch.DrawString(font_items, item_titles[i], pos + size_item / 2 - title_sizes[i] / 2 + new Vector2(0, size_item.Y * (i + 1) + 2), col_items_text);
                    }
                }
            }

            Game1.DrawRectangle(pos - new Vector2(2), size_item + new Vector2(4, 4), col_edge, 2);
            if (state == 1)
                Game1.DrawRectangle(pos - new Vector2(2), size_item + new Vector2(4, 6 + size_item.Y * itemnum), col_edge, 2);
            Game1.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
