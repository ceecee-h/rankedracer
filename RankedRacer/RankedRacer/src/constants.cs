using Microsoft.Xna.Framework.Graphics;
using RankedRacer.src.entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RankedRacer.src
{
    public static class constants
    {
        public static Vector2 bkgVelocity = new Vector2(160, 0);
        public static int numObstacles = 3;
        public static Vector2 baseScreenSize = new Vector2(400, 350);
        public static int NA = -1;
        public static string UNKNOWN = "UNKNOWN";
        public static float textSpeed = 0.05f;
        public static Vector2 ballotTextStart = new Vector2(260, 220);
        public static Vector2 matrixTextStart = new Vector2(52, 45);
        public static int matrixY = 30;
        public static int matrixX = 35;
        public static float textHeight = 15;
        public static Vector2 totalVoters = new Vector2(75, 218);
        public static List<GameState> staticStates = new List<GameState>{GameState.Menu, GameState.Paused, GameState.Ballots};


        private static readonly string[] chars = { "player", "player", "v", "v", "mateo", "mateo", "v", "v", "player", "player", "player" };
        private static readonly string[] texts =
        {
            "This is the fifth time \nthis week my art",
            "Has been painted over!\nSomething needs to change",
            "Hey Mateo!, Did you \nhear about the election?",
            "The Street Art Coalition\n election is soon...",
            "Yep! They say whoever is \nelected president",
            "could change our co-op's \nart rules in this city",
            "The co-op is trying out \nInstant Run-off Voting this",
            "year! I wonder how \nthat will go!",
            "Hmmm...",
            "I need to get \nelected president!",
            "At all costs."
        };


        public static sequence intro = new sequence(
            chars,
            texts
            );

        private static readonly string[] timesUpChars = { "player", "player" };
        private static readonly string[] timesUpTexts =
        {
            "The election is starting\nsoon...",
            "These ballots will have to do",
            "I need to get to\n The Street Art Coalition!"
        };


        public static sequence timesUp = new sequence(
            timesUpChars,
            timesUpTexts
            );

        private static readonly string[] rigChars = { "player", "player" };
        private static readonly string[] rigText =
        {
            "I need to get\nout of here!",
            "Before someone catches\nme!"
        };


        public static sequence rig = new sequence(
            rigChars,
            rigText
            );


        private static readonly string[] failChars = { "polly", "polly", "polly", "polly" };
        private static readonly string[] failText =
        {
            "Ha! I caught you!",
            "I can't believe you\ntried to rig the",
            "election.\nYou are officially...",
            "Banned from the Coalition!"
        };


        public static sequence failS = new sequence(
            failChars,
            failText
            );

        private static readonly string[] successChars = { "player", "player", "player"};
        private static readonly string[] successText =
        {
            "Yes! No one saw me!",
            "I will surely win the\nelection now.",
            "No one will cover\nmy art ever again.",
        };


        public static sequence success = new sequence(
            successChars,
            successText
            );
    }

    public class sequence
    {
        // list of dialog, name, text
        public List<(string, string)> dialogs;
        private int next;

        public sequence(string[] chars, string[] texts)
        {
            dialogs = new List<(string, string)>();
            for (int i = 0; i < chars.Length; i++)
            {
                dialogs.Add((chars[i], texts[i]));
            }
            next = 0;
        }

        public bool isIterated()
        {
            return next >= dialogs.Count;
        }

        public (string, string) Next()
        {
            if (next >= dialogs.Count) return (null, null);
            (string, string) rt = dialogs[next];
            next++;
            return rt;
        }
    }
}
