using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RankedRacer.src.entities
{
    public class Puzzle : Entity
    {
        ContentManager content;
        List<Ballot> ballots;
        EMatrix matrix;
        List<UnknownButton> unknownButtons;
        int currentBallot;
        List<int> currentUnknowns;
        SpriteFont font;
        Rectangle submitBounds = new Rectangle(73, 285, 53, 18);
        Rectangle qBounds = new Rectangle(167, 317, 15, 15);
        bool showInstruction;
        Texture2D instructions;

        public event MatrixHandler matrixUpdated;
        public event EndHandler submitMatrix;
        public Puzzle(IServiceProvider serviceProvider, List<Ballot> ballots, string[] candidates)
        {
            content = new ContentManager(serviceProvider, "Content");
            sprite = content.Load<Texture2D>("4_puzzle");
            this.ballots = ballots;
            this.unknownButtons = new List<UnknownButton>();
            currentBallot = 0;
            currentUnknowns = new List<int>();
            font = content.Load<SpriteFont>("RetroGaming");
            showInstruction = true;
            instructions = content.Load<Texture2D>("how_to_puzzle");
            for (int i = 0; i < ballots.Count; i++)
            {
                List<string> missing_candidates = new List<string>();
                missing_candidates = candidates.Where(x => !ballots[i].ranking.Contains(x)).ToList();
                foreach (int j in ballots[i].unknownVals())
                {
                    unknownButtons.Add(new UnknownButton(new Vector2(constants.ballotTextStart.X, constants.ballotTextStart.Y + (constants.textHeight * j)), font, i, j, missing_candidates));
                }
            }
            matrix = null;
        }

        public void setMatrix(EMatrix m) { matrix = m; }

        public int getButton(int ballot, int position)
        {
            for (int i = 0; i < unknownButtons.Count; i++)
            {
                if (unknownButtons[i].ballotAssociation == ballot && unknownButtons[i].rankingPosition == position)
                {
                    return i;
                }
            }
            return -1;
        }

        public void setUnknowns()
        {
            currentUnknowns = new List<int>();
            for (int i = 0; i < unknownButtons.Count; i++)
            {
                if (unknownButtons[i].ballotAssociation == currentBallot)
                {
                    currentUnknowns.Add(i);
                }
            }
        }

        public List<Ballot> getCurrentList()
        {
            List<Ballot> ls = new List<Ballot>();
            for (int i = 0; i < ballots.Count; i++)
            {
                string[] ranking = new string[ballots[i].ranking.Length];
                ballots[i].ranking.CopyTo(ranking, 0);
                foreach (int j in ballots[i].unknownVals())
                {
                    ranking[j] = unknownButtons[getButton(i, j)].currentCandidate;
                }
                ls.Add(new Ballot(ballots[i].tally, ranking));
            }
            return ls;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Vector2(0, 0), Color.White);
            if (showInstruction)
            {
                spriteBatch.Draw(instructions, new Vector2(0, 0), Color.White);
                return;
            }
            Ballot b = ballots[currentBallot];
            for (int i = 0; i < b.ranking.Length; i++)
            {
                Vector2 placement = new Vector2(constants.ballotTextStart.X, constants.ballotTextStart.Y + (constants.textHeight * i));
                if (b.ranking[i].Equals(constants.UNKNOWN))
                {
                    unknownButtons[getButton(currentBallot, i)].Draw(spriteBatch);
                }
                else spriteBatch.DrawString(font, (i+1).ToString()+ ". " + b.ranking[i], placement, Color.Black);
            }
            spriteBatch.DrawString(font, b.tally.ToString(), new Vector2(constants.ballotTextStart.X + 20, constants.ballotTextStart.Y + (constants.textHeight * (b.ranking.Count()+1))), Color.Black);

            if (matrix == null)
            {
                matrixUpdated(ballots);
            }
            //matrix.solve();
            for (int i = 0; i < matrix.matrix.Length; i++)
            {
                string abv = matrix.key_map.FirstOrDefault(x => x.Value == i).Key;
                abv = abv.Length > 1 ? abv.Substring(0, 2) + "." : abv;
                spriteBatch.DrawString(font, abv, 
                    constants.matrixTextStart + new Vector2(-constants.matrixX, i * constants.matrixY), Color.White);
                for (int j = 0; j < matrix.matrix[i].Length; j++)
                {
                    Vector2 adj = new Vector2(j * constants.matrixX, i * constants.matrixY);
                    spriteBatch.DrawString(font, matrix.matrix[i][j].ToString(), constants.matrixTextStart + adj, Color.White);
                }
            }
            spriteBatch.DrawString(font, matrix.total_votes.ToString(), constants.totalVoters, Color.White);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gt)
        {
            if (showInstruction && MyMouse.HasBeenPressed())
            {
                showInstruction = false;
            }
            if (KeyBoard.HasBeenPressed(Keys.Right) || KeyBoard.HasBeenPressed(Keys.D))
            {
                if (currentBallot < ballots.Count - 1)
                {
                    currentBallot++;
                    setUnknowns();
                }
            }
            if (KeyBoard.HasBeenPressed(Keys.Left) || KeyBoard.HasBeenPressed(Keys.A))
            {
                if (currentBallot > 0)
                {
                    currentBallot--;
                    setUnknowns();
                }
            }
            if (MyMouse.Clicked(qBounds))
            {
                showInstruction = true;
            }

            foreach (int i in currentUnknowns)
            {
                if (MyMouse.Clicked(unknownButtons[i].bounds))
                {
                    unknownButtons[i].updateOption();
                    matrixUpdated(getCurrentList());
                }
            }

            if (MyMouse.Clicked(submitBounds)) submitMatrix();
        }
    }

    public class UnknownButton
    {
        public string currentCandidate;
        Vector2 position;
        SpriteFont font;
        List<string> options;
        int index;
        public Rectangle bounds;
        public int ballotAssociation;
        public int rankingPosition;

        public UnknownButton(Vector2 pos, SpriteFont font, int ballotAssociation, int rankingPosition, List<string> options)
        {
            currentCandidate = constants.UNKNOWN;
            position = pos;
            this.font = font;
            this.index = 0;
            bounds = new Rectangle((int)position.X, (int)position.Y, 80, (int)constants.textHeight);
            this.ballotAssociation = ballotAssociation;
            this.rankingPosition = rankingPosition;
            this.options = options;
        }

        public void updateOption()
        {
            System.Diagnostics.Debug.WriteLine("Update Option");
            currentCandidate = options[index];
            index++;
            if (index >= options.Count) {
                index = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.Black });
            spriteBatch.Draw(rect, bounds,
                Color.Black);
            if (!currentCandidate.Equals(constants.UNKNOWN))
            {
                spriteBatch.DrawString(font, (rankingPosition + 1).ToString() + ". " + currentCandidate, new Vector2(position.X + 2, position.Y-2), Color.White);
            } else
            {
                spriteBatch.DrawString(font, (rankingPosition + 1).ToString() + ". ", new Vector2(position.X + 2, position.Y-2), Color.White);
            }
        }
    }
}
