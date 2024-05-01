using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankedRacer.src
{
    public class Logic
    {
        private static string[] cnames = { "Dory", "Simon", "Polly", "Mateo", "V", "Fiona"};

        private int num_voters;
        private int num_candidates;
        public List<Ballot> player_ballots;
        private int next_ballot = 0;
        public Ballot[] obscured_ballots;
        public Ballot[] ballots;
        public string[] candidates;
        public EMatrix electionMatrix;
        Random rand = new Random();

        public event StateHandler changeState;
        private string[] gen_ranking()
        {
            return candidates.OrderBy(_ => rand.Next()).Take(5).ToArray();
        }

        private int[] partition(int n)
        {
            int range_v = num_voters + (n-1);
            int[] p = Enumerable.Range(0, range_v).ToArray().OrderBy(_ => rand.Next()).Take(n-1).ToArray();
            List<int> partitions = new List<int>(p);
            for (int i = 0; i < p.Length; i++)
            {
                partitions.Add(p[i] + 1);
            }
            partitions.Add(range_v);
            partitions.Add(0);
            partitions.Sort();
            int[] result = new int[6];
            int a = 0;
            int x = 0;
            int y = 1;
            while (y < partitions.Count) {
                result[a] = partitions[y] - partitions[x];
                x += 2;
                y += 2;
                a++;
            }
            return result;
        }

        public void genELection()
        {
            ballots = new Ballot[5];
            num_voters = rand.Next(20, 150);
            num_candidates = 4;//rand.Next(3, 6);
            List<string> dup = cnames.OrderBy(_ => rand.Next()).Take(num_candidates-1).ToList();
            dup.Add("Player");
            candidates = dup.ToArray();
            List<string[]> rankings = new List<string[]>();
            for (int i = 0; i < 5; i++)
            {
                rankings.Add(gen_ranking());
            }
            rankings = rankings.Distinct().ToList();
            int[] partitions = partition(rankings.Count);
            
            for (int i = 0; i < rankings.Count; i++)
            {
                Ballot b = new Ballot(partitions[i], rankings[i]);
                ballots[i] = b;
            }
            Debug.WriteLine("Generated ELection");
            player_ballots = new List<Ballot>();
            electionMatrix = new EMatrix(ballots, candidates, num_voters);
            obscureElection();
        }

        private void obscureElection()
        {
            obscured_ballots = new Ballot[ballots.Length];
            Ballot[] temp = new Ballot[ballots.Length];
            foreach (Ballot b in ballots)
            {
                obscured_ballots.Append(b.Clone());
                temp.Append(b.Clone());

            }
            ballots.CopyTo(temp, 0);
            EMatrix m = new EMatrix(ballots, candidates, num_voters);
            
            while (m.solve())
            {
                temp.CopyTo(obscured_ballots, 0);
                int i = rand.Next(0, numBallots());
                temp[i] = temp[i].randomObscure();
                m = new EMatrix(temp, candidates, num_voters);
                System.Diagnostics.Debug.WriteLine(m);
            }
            System.Diagnostics.Debug.WriteLine("Obscured Election: ");
            foreach (Ballot b in obscured_ballots)
            {
                System.Diagnostics.Debug.WriteLine(b.ToString());
            }
        }

        public Ballot foundBallot()
        {
            Ballot new_ballot = obscured_ballots[next_ballot];
            player_ballots.Add(new_ballot);
            next_ballot += 1;
            if (numPlayerBallots() == numBallots())
            {
                changeState(GameState.Puzzle);
            }

            return new_ballot;
        }

        public bool solve(Ballot[] bs)
        {
            EMatrix m = new EMatrix(bs, candidates, num_voters);
            return m.solve();
        }

        public void reset()
        {
            player_ballots = new List<Ballot>();
            next_ballot = 0;
        }

        public int numBallots() { return obscured_ballots.Length; }

        public int numPlayerBallots() { return player_ballots.Count; }

        public int numVoters() { return num_voters; }
    }
    public class Ballot
    {
        public int tally;
        public string[] ranking;
        public int key;
        public bool[] unknowns;

        public Ballot(int tally, string[] ranking)
        {
            this.tally = tally;
            this.ranking = ranking;
            unknowns = new bool[ranking.Length];
            foreach (string s in ranking)
            {
                unknowns.Append(s.Equals(constants.UNKNOWN));
            }
        }

        public Ballot randomObscure()
        {
            Random rand = new Random();
            string[] new_ranking = new string[ranking.Length];
            ranking.CopyTo(new_ranking, 0);

            int obscureIndex = rand.Next(0, ranking.Length - 1);
            new_ranking[obscureIndex] = constants.UNKNOWN;
            return new Ballot(tally, new_ranking);
        }

        public List<int> unknownVals()
        {
            List<int> unknowns = new List<int>();
            for (int i  = 0; i < ranking.Length; i++)
            {
                if (ranking[i].Equals(constants.UNKNOWN)) {
                    unknowns.Add(i);
                };
            }
            return unknowns;
        }

        public String formatStr()
        {
            return string.Join("\n", ranking) + "\n\n Tally " + tally;
        }

        public override string ToString()
        {
            return "Ranking: " + string.Join(", ", ranking) + "\n\tTally : " + tally;
        }

        public Ballot Clone()
        {
            string[] new_ranking = new string[ranking.Length];
            for (int i = 0; i < ranking.Length; i++)
            {
                new_ranking[i] = ranking[i];
            }
            return new Ballot(tally, new_ranking);
        }
    }

    public class EMatrix
    {
        public int[][] matrix;
        public string[] labels;
        public Dictionary<string, int> key_map;
        public Ballot[] ballots;
        public int total_votes;
        private int rows;
        private int cols;

        public EMatrix(Ballot[] bs, string[] candidates, int total_votes)
        {
            this.labels = candidates;
            this.key_map = new Dictionary<string, int>();
            this.total_votes = total_votes;
            this.ballots = bs;
            rows = labels.Length;
            cols = bs[0].ranking.Length;
            this.matrix = new int[rows][];
            for (int i = 0; i < rows; i++) {
                matrix[i] = new int[cols];
            }

            for (int i = 0; i < rows; i++) { key_map[labels[i]] = i; }
            foreach (Ballot b in bs)
            {
                int pos = 0;
                foreach (string c in b.ranking) {
                    if (!c.Equals(constants.UNKNOWN) && b.tally != constants.NA)
                    {
                        matrix[key_map[c]][pos] += b.tally;
                    }
                    pos++;
                }
            }
        }

        public int getColSum(int i)
        {
            int sum = 0;
            for (int j = 0; j < rows; j++)
            {
                if (matrix[j][i] != constants.NA)
                    sum += matrix[j][i];
            }
            return sum;
        }

        public int getRowSum(int i)
        {
            int sum = 0;
            for (int j = 0; j < rows; j++)
            {
                if (matrix[i][j] != constants.NA)
                    sum += matrix[i][j];
            }
            return sum;
        }

        public bool solve()
        {
            List<Ballot> bs = new List<Ballot>();
            foreach (Ballot b in ballots)
            {
                bs.Add(b.Clone());
            }
            int prev = bs.Count;
            while (!isCorrect())
            {
                for (int i = 0; i < bs.Count; i++)
                {
                    List<int> unknowns = bs[i].unknownVals();
                    // add case if tally obscured here
                    if (unknowns.Count == 0) bs.RemoveAt(i);
                    else
                    {
                        foreach (int u in unknowns)
                        {
                            foreach (string val in labels.Where(x => !bs[i].ranking.Contains(x)))
                            {
                                if ((getColSum(u) + bs[i].tally == total_votes) && (getRowSum(key_map[val]) + bs[i].tally == total_votes))
                                {
                                    matrix[key_map[val]][u] += bs[i].tally;
                                    bs[i].ranking[u] = val;
                                }
                            }
                        }
                    }
                }
                if (bs.Count == 0) break;
                if (bs.Count == prev) return false;
                prev = bs.Count;
            }
            return isCorrect();
        }

        public bool isCorrect() {
            // correctly solved?
            foreach (string i in labels)
            {
                if (getRowSum(key_map[i]) != total_votes) return false;
            }

            for (int i = 0; i < cols; i++)
            {
                if (getColSum(i) != total_votes) return false;
            }
            return true;
        }

        public bool matchesMatrix(EMatrix other)
        {
            if (this.cols != other.cols || this.rows != other.rows) return false;
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    if (this.matrix[i][j] != other.matrix[i][j]) return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            String rt = "";
            int x = 0;
            foreach (int[] i in matrix)
            {
                rt += labels[x] + "\t";
                foreach (int j in i)
                {
                    rt += j.ToString() + "\t";
                }
                rt += "\n";
                x++;
            }
            return rt;
        }
    }
}
