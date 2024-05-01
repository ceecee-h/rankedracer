using RankedRacer.src.entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RankedRacer.src
{
    public static class IRV
    {

        private static (Dictionary<string, int>, int) irv_round(List<Ballot> ballots, List<string> candidates)
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();
            foreach (string s in candidates)
            {
                scores[s] = 0;
            }
            List<Ballot> remove_ballots = new List<Ballot> ();
            int remove_votes = 0;
            foreach (Ballot ballot in ballots) {
                bool eliminate = true;
                for (int i = 0; i < ballot.ranking.Length; i++) {
                    if (candidates.Contains(ballot.ranking[i])) {
                        eliminate = false;
                        scores[ballot.ranking[i]] += ballot.tally;
                        break;
                    }
                    if (eliminate) remove_ballots.Add(ballot);
                } 
            }
            foreach (Ballot b in remove_ballots) {
                ballots.Remove(b);
                remove_votes += b.tally;
            }
            return (scores, remove_votes);
        }

        public static string runIRV(List<Ballot> ballots, List<string> candidates, int total_votes) {

            int rmv;
            Dictionary<string, int> scores;
            (scores, rmv) = irv_round(ballots, candidates);
            int max_score = scores.Values.Max();
            total_votes -= rmv;
            while (max_score <= (total_votes / 2)) {
                int min_score = scores.Values.Min();
                string[] losers = scores.Keys.Where(x => scores[x] == min_score).ToArray();
                if (losers.Count() > 1) return constants.UNKNOWN;
                candidates.Remove(losers[0]);
                if (candidates.Count() == 1) return candidates[0];

                (scores, rmv) = irv_round(ballots, candidates);
                max_score = scores.Values.Max();
                total_votes -= rmv;
                if (max_score == 0) return constants.UNKNOWN;
            }
            string[] winners = scores.Keys.Where(x => scores[x] == max_score).ToArray();
            return winners.Length == 1 ? winners[0] : constants.UNKNOWN;
        }
    }
}
