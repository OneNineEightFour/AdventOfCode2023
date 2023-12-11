using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

#pragma warning disable CS8321

var sw = Stopwatch.StartNew();
var result = Day11_1();
sw.Stop();

Console.WriteLine(result);
Console.WriteLine($"{(int)sw.Elapsed.TotalMilliseconds} ms");

static int Day1_1()
{
    return File.ReadAllLines("data/Day1.txt")
        .Select(line => line.First(char.IsDigit) + "" + line.Last(char.IsDigit))
        .Sum(int.Parse);
}

static int Day1_2()
{
    var mapping = new Dictionary<string, int>() {
        {"1", 1},
        {"2", 2},
        {"3", 3},
        {"4", 4},
        {"5", 5},
        {"6", 6},
        {"7", 7},
        {"8", 8},
        {"9", 9},
        {"one", 1},
        {"two", 2},
        {"three", 3},
        {"four", 4},
        {"five", 5},
        {"six", 6},
        {"seven", 7},
        {"eight", 8},
        {"nine", 9},
    };
    var matcher = string.Join('|', mapping.Keys);

    return File.ReadAllLines("data/Day1.txt")
        .Select(line => (first: Regex.Match(line, matcher).Value, last: Regex.Match(line, matcher, RegexOptions.RightToLeft).Value))
        .Select(item => mapping[item.first] * 10 + mapping[item.last])
        .Sum();
}

static int Day2_1()
{
    return File.ReadLines("data/Day2.txt")
        .Select(MapGame)
        .Where(game => game.sets.All(set => set.All(Condition)))
        .Sum(game => game.gameId);

    static (int gameId, IEnumerable<IEnumerable<(int count, string color)>> sets) MapGame(string line) =>
        Map(line.Split(':', StringSplitOptions.TrimEntries), values => (int.Parse(values[0].Split(' ')[1]), MapSet(values[1])));

    static IEnumerable<IEnumerable<(int count, string color)>> MapSet(string set) =>
        set.Split(';', StringSplitOptions.TrimEntries)
            .Select(set => set.Split(',', StringSplitOptions.TrimEntries).Select(MapBalls));

    static (int count, string color) MapBalls(string set) =>
        Map(set.Split(' '), values => (int.Parse(values[0]), values[1]));

    static bool Condition((int count, string color) ball) =>
        ball switch
        {
            ( > 12, "red") => false,
            ( > 13, "green") => false,
            ( > 14, "blue") => false,
            _ => true,
        };

    static T Map<T>(string[] self, Func<string[], T> mapper) =>
        mapper(self);
}

static int Day2_2()
{
    return File.ReadLines("data/Day2.txt")
        .Select(MapGame)
        .Select(game => game.sets.SelectMany(set => set).ToImmutableArray())
        .Select(sets => (MaxBallCount(sets, "red"), MaxBallCount(sets, "green"), MaxBallCount(sets, "blue")))
        .Sum(item => item.Item1 * item.Item2 * item.Item3);

    static (int gameId, IEnumerable<IEnumerable<(int count, string color)>> sets) MapGame(string line) =>
        Map(line.Split(':', StringSplitOptions.TrimEntries), values => (int.Parse(values[0].Split(' ')[1]), MapSet(values[1])));

    static IEnumerable<IEnumerable<(int count, string color)>> MapSet(string set) =>
        set.Split(';', StringSplitOptions.TrimEntries)
            .Select(set => set.Split(',', StringSplitOptions.TrimEntries).Select(MapBalls));

    static (int count, string color) MapBalls(string set) =>
        Map(set.Split(' '), values => (int.Parse(values[0]), values[1]));

    static int MaxBallCount(IEnumerable<(int count, string color)> sets, string color) =>
        sets.Where(ball => ball.color == color).Max(ball => ball.count);

    static T Map<T>(string[] self, Func<string[], T> mapper) =>
        mapper(self);
}

static int Day3_1()
{
    var input = File.ReadAllLines("data/Day3.txt");

    var numbers = FindObjects(input, "\\d+");
    var cogs = FindObjects(input, "[^\\.\\d]");

    return numbers
        .Where(number => cogs.Any(cog => Overlaps(number, cog)))
        .Sum(number => int.Parse(number.value));

    static IEnumerable<(int x, int y, string value)> FindObjects(string[] lines, string regex) =>
        lines
            .Select(line => Regex.Matches(line, regex).Cast<Match>())
            .SelectMany((matches, i) => matches.Select(match => (x: match.Index, y: i, value: match.Value)));

    static bool Overlaps((int x, int y, string value) number, (int x, int y, string value) cog) =>
        Math.Abs(number.y - cog.y) <= 1 && cog.x >= number.x - 1 && cog.x <= number.x + number.value.Length;
}

static int Day3_2()
{
    var input = File.ReadAllLines("data/Day3.txt");

    var numbers = FindObjects(input, "\\d+");
    var cogs = FindObjects(input, "\\*");

    return cogs
        .Select(cog => (first: numbers.First(number => Overlaps(number, cog)), second: numbers.Last(number => Overlaps(number, cog))))
        .Where(numberPair => numberPair.first != numberPair.second)
        .Sum(numberPair => int.Parse(numberPair.first.value) * int.Parse(numberPair.second.value));

    static IEnumerable<(int x, int y, string value)> FindObjects(string[] lines, string regex) =>
        lines
            .Select(line => Regex.Matches(line, regex).Cast<Match>())
            .SelectMany((matches, i) => matches.Select(match => (x: match.Index, y: i, value: match.Value)));

    static bool Overlaps((int x, int y, string value) number, (int x, int y, string value) cog) =>
        Math.Abs(number.y - cog.y) <= 1 && cog.x >= number.x - 1 && cog.x <= number.x + number.value.Length;
}

static int Day4_1()
{
    return File.ReadLines("data/Day4.txt")
        .Select(line => line.Split(':', '|'))
        .Select(lineParts => (cardId: FindNumbers(lineParts[0]).First(), winning: FindNumbers(lineParts[1]), gotten: FindNumbers(lineParts[2])))
        .Select(card => card.gotten.Intersect(card.winning).Count())
        .Select(cardWonCount => cardWonCount > 0 ? (int)Math.Pow(2, cardWonCount - 1) : 0)
        .Sum();

    static IEnumerable<int> FindNumbers(string text) =>
        Regex.Matches(text, "\\d+")
            .Select(match => int.Parse(match.Value))
            .ToImmutableArray();
}

static int Day4_2()
{
    return File.ReadLines("data/Day4.txt")
        .Select(line => line.Split(':', '|'))
        .Select(lineParts => (cardId: FindNumbers(lineParts[0]).First(), winning: FindNumbers(lineParts[1]), gotten: FindNumbers(lineParts[2])))
        .Select(card => (cardId: card.cardId, won: card.gotten.Intersect(card.winning).Count()))
        .Aggregate(Enumerable.Empty<int>(), (accum, curr) => accum.Append(curr.cardId).Concat(RepeatValues(Enumerable.Range(curr.cardId + 1, curr.won), accum.Count(cardId => curr.cardId == cardId) + 1)).ToImmutableArray())
        .Count();

    static IEnumerable<int> FindNumbers(string text) =>
        Regex.Matches(text, "\\d+")
            .Select(match => int.Parse(match.Value))
            .ToImmutableArray();

    static IEnumerable<T> RepeatValues<T>(IEnumerable<T> values, int count) =>
        Enumerable.Repeat(values, count).SelectMany(cardId => cardId);
}

static long Day5_1()
{
    var input = File.ReadAllText("data/Day5.txt")
        .Split(Environment.NewLine + Environment.NewLine);

    var seeds = Regex.Matches(input.First(), "\\d+")
        .Select(match => long.Parse(match.Value))
        .ToImmutableArray();

    var maps = input
        .Skip(1)
        .Select(mapData => mapData.Split(Environment.NewLine))
        .Select(mapData => (name: mapData.First(), ranges: mapData.Skip(1).Select(line => line.Split(' ').Select(long.Parse).ToImmutableArray())))
        .Select(mapData => (name: mapData.name, ranges: mapData.ranges.Select(range => (destination: range[0], source: range[1], range: range[2]))))
        .ToImmutableArray();

    return seeds
        .Min(seed => maps.Aggregate(seed, (location, map) => FindLocation(map, location)));

    static long FindLocation((string name, IEnumerable<(long destination, long source, long length)> ranges) map, long location)
    {
        var range = map.ranges.FirstOrDefault(range => location >= range.source && location < range.source + range.length);
        return location + (range.destination - range.source);
    }
}

static long Day5_2()
{
    throw new NotImplementedException();
}

static int Day6_1()
{
    var data = File.ReadAllLines("data/Day6.txt")
        .Select(line => Regex.Matches(line, "\\d+").Select(match => int.Parse(match.Value)));

    return data
        .First()
        .Zip(data.Last())
        .Select(race => GetRaceDistances(race).Count(distance => distance > race.Second))
        .Aggregate(1, (accum, curr) => accum * curr);

    static IEnumerable<int> GetRaceDistances((int time, int distance) race) =>
        Enumerable.Range(1, race.time)
            .Select(time => (race.time - time) * time);
}

static int Day6_2()
{
    var input = File.ReadAllLines("data/Day6.txt");

    var targetTime = long.Parse(input[0].Where(char.IsDigit).ToArray());
    var targetDistance = long.Parse(input[1].Where(char.IsDigit).ToArray());

    return Enumerable.Range(14, (int)targetTime - 14)
        .Select(time => (targetTime - time) * time)
        .Count(distance => distance > targetDistance);
}

static int Day7_1()
{
    const string Cards = "AKQJT98765432";

    return File.ReadLines("data/Day7.txt")
        .Select(line => line.Split(' '))
        .Select(line => (cards: line[0], bid: int.Parse(line[1])))
        .OrderBy(hand => CalculateCardTypeStrength(hand.cards))
        .ThenByDescending(hand => CalculateHandStrength(hand.cards))
        .Select((hand, i) => hand.bid * (i + 1))
        .Sum();

    static int CalculateCardTypeStrength(string hand) =>
        hand
            .GroupBy(c => c)
            .Select(cardGroup => (cardType: cardGroup.Key, count: cardGroup.Count()))
            .OrderByDescending(cardGroup => cardGroup.count)
            .ToImmutableArray() switch
        {
        [(_, 5)] => 7,
        [(_, 4), (_, 1)] => 6,
        [(_, 3), (_, 2)] => 5,
        [(_, 3), (_, 1), (_, 1)] => 4,
        [(_, 2), (_, 2), (_, 1)] => 3,
        [(_, 2), (_, 1), (_, 1), (_, 1)] => 2,
        [(_, 1), (_, 1), (_, 1), (_, 1), (_, 1)] => 1,
            _ => throw new UnreachableException(),
        };

    static int CalculateHandStrength(string hand) =>
        hand.Aggregate(1, (totalStrength, card) => totalStrength * Cards.Length + Cards.IndexOf(card));

}

static int Day7_2()
{
    const string Cards = "AKQT98765432J";

    return File.ReadLines("data/Day7.txt")
        .Select(line => line.Split(' '))
        .Select(line => (cards: line[0], bid: int.Parse(line[1])))
        .OrderBy(hand => CalculateCardTypeStrength(hand.cards))
        .ThenByDescending(hand => CalculateHandStrength(hand.cards))
        .Select((hand, i) => hand.bid * (i + 1))
        .Sum();

    static int CalculateCardTypeStrength(string hand) =>
        hand
            .Replace('J', FindBestJokerReplacement(hand))
            .GroupBy(c => c)
            .Select(cardGroup => (cardType: cardGroup.Key, count: cardGroup.Count()))
            .OrderByDescending(cardGroup => cardGroup.count)
            .ToImmutableArray() switch
        {
        [(_, 5)] => 7,
        [(_, 4), (_, 1)] => 6,
        [(_, 3), (_, 2)] => 5,
        [(_, 3), (_, 1), (_, 1)] => 4,
        [(_, 2), (_, 2), (_, 1)] => 3,
        [(_, 2), (_, 1), (_, 1), (_, 1)] => 2,
        [(_, 1), (_, 1), (_, 1), (_, 1), (_, 1)] => 1,
            _ => throw new UnreachableException(),
        };

    static int CalculateHandStrength(string hand) =>
        hand.Aggregate(1, (totalStrength, card) => totalStrength * Cards.Length + Cards.IndexOf(card));

    static char FindBestJokerReplacement(string hand) =>
        hand
            .Where(c => c != 'J')
            .GroupBy(c => c)
            .MaxBy(group => group.Count())?.Key ?? '0';
}

static int Day8_1()
{
    var input = File.ReadAllLines("data/Day8.txt");

    var directions = input.First();
    var graph = input
        .Skip(2)
        .Select(line => Regex.Matches(line, "\\w+").Select(match => match.Value).ToImmutableArray())
        .Select(ids => (id: ids[0], left: ids[1], right: ids[2]))
        .ToFrozenDictionary(node => node.id, node => (left: node.left, right: node.right));

    return FindEndNode(graph, (0, "AAA"), directions).steps;

    static (int steps, string id) FindEndNode(IDictionary<string, (string left, string right)> graph, (int steps, string id) initialState, string directions) =>
        !IsEndNode(initialState.id) ?
            FindEndNode(graph, directions.Aggregate(initialState, (state, direction) => !IsEndNode(state.id) ? (state.steps + 1, NextNode(graph, state.id, direction)) : state), directions) :
            initialState;

    static bool IsEndNode(string id) =>
        id == "ZZZ";

    static string NextNode(IDictionary<string, (string left, string right)> graph, string id, char direction) =>
        direction == 'L' ?
            graph[id].left :
            graph[id].right;
}

static int Day8_2()
{
    throw new NotImplementedException();
}

static int Day9_1()
{
    return File.ReadAllLines("data/Day9.txt")
        .Select(line => line.Split(' ').Select(int.Parse))
        .Select(line => CalculateNextValue(line))
        .Sum();

    static int CalculateNextValue(IEnumerable<int> values) =>
        values.Any(value => value != 0) ?
            values.Last() + CalculateNextValue(CalculateDifference(values)) :
            0;

    static IEnumerable<int> CalculateDifference(IEnumerable<int> values) =>
        values
            .Zip(values.Skip(1))
            .Select(item => item.Second - item.First);
}

static int Day9_2()
{
    return File.ReadAllLines("data/Day9.txt")
        .Select(line => line.Split(' ').Select(int.Parse))
        .Select(line => CalculatePreviousValue(line))
        .Sum();

    static int CalculatePreviousValue(IEnumerable<int> values) =>
        values.Any(value => value != 0) ?
            values.First() - CalculatePreviousValue(CalculateDifference(values)) :
            0;

    static IEnumerable<int> CalculateDifference(IEnumerable<int> values) =>
        values
            .Zip(values.Skip(1))
            .Select(item => item.Second - item.First);
}

static int Day10_1()
{
    var pipes = File.ReadAllLines("data/Day10.txt")
        .SelectMany((line, y) => line.Select((c, x) => (x: x, y: y, c: c)))
        .Where(obj => obj.c != '.');

    var start = pipes.First(pipe => pipe.c == 'S');

    return FindPipeLine(pipes, start).Count() / 2;

    static IEnumerable<(int x, int y, char c)> FindPipeLine(IEnumerable<(int x, int y, char c)> pipes, (int x, int y, char c) start) =>
        start != default ?
            FindPipeLine(pipes.Where(pipe => pipe != start).ToImmutableArray(), FindFirstConnectingPipe(pipes, start)).Prepend(start) :
            Enumerable.Empty<(int x, int y, char c)>();

    static (int x, int y, char c) FindFirstConnectingPipe(IEnumerable<(int x, int y, char c)> pipes, (int x, int y, char c) start) =>
        pipes
            .Where(pipe => Math.Abs(pipe.x - start.x) + Math.Abs(pipe.y - start.y) == 1)
            .FirstOrDefault(pipe => IsValidConnection(start, pipe));

    static bool IsValidConnection((int x, int y, char c) first, (int x, int y, char c) second) =>
        (second.x - first.x, second.y - first.y, first.c) switch
        {
            (-1, 0, '-') => second.c is '-' or 'L' or 'F',
            (-1, 0, 'J') => second.c is '-' or 'L' or 'F',
            (-1, 0, '7') => second.c is '-' or 'L' or 'F',
            (1, 0, '-') => second.c is '-' or '7' or 'J',
            (1, 0, 'F') => second.c is '-' or '7' or 'J',
            (1, 0, 'L') => second.c is '-' or '7' or 'J',
            (0, -1, '|') => second.c is '|' or 'F' or '7',
            (0, -1, 'J') => second.c is '|' or 'F' or '7',
            (0, -1, 'L') => second.c is '|' or 'F' or '7',
            (0, 1, '|') => second.c is '|' or 'L' or 'J',
            (0, 1, 'F') => second.c is '|' or 'L' or 'J',
            (0, 1, '7') => second.c is '|' or 'L' or 'J',
            (-1, 0, 'S') => second.c is '-' or 'L' or 'F',
            (1, 0, 'S') => second.c is '-' or '7' or 'J',
            (0, -1, 'S') => second.c is '|' or 'F' or '7',
            (0, 1, 'S') => second.c is '|' or 'L' or 'J',
            _ => false,
        };
}

static int Day10_2()
{
    throw new NotImplementedException();
}

static int Day11_1()
{
    var universe = File.ReadAllLines("data/Day11.txt")
        .SelectMany((line, y) => line.Select((c, x) => (x: x, y: y, c: c)))
        .Where(space => space.c == '#');

    return GetPermutations(Expand(universe))
        .Select(galaxyPair => CalculateDistance(galaxyPair.first, galaxyPair.second))
        .Sum();

    static IEnumerable<(T first, T second)> GetPermutations<T>(IEnumerable<T> elements) =>
        elements.Any() ?
            elements
                .Skip(1)
                .Select(element => (elements.First(), element))
                .Concat(GetPermutations(elements.Skip(1))) :
            Enumerable.Empty<(T, T)>();

    static IEnumerable<(int x, int y, char c)> Expand(IEnumerable<(int x, int y, char c)> universe) =>
        universe
            .GroupBy(val => val.y)
            .OrderBy(group => group.Key)
            .SelectMany((group, i) => group.Select(galaxy => (x: galaxy.x, y: galaxy.y + (group.Key - i), c: '#')))
            .GroupBy(val => val.x)
            .OrderBy(group => group.Key)
            .SelectMany((group, i) => group.Select(galaxy => (x: galaxy.x + (group.Key - i), y: galaxy.y, c: '#')))
            .ToImmutableArray();

    static int CalculateDistance((int x, int y, char c) first, (int x, int y, char c) second) =>
        Math.Abs(first.x - second.x) + Math.Abs(first.y - second.y);
}