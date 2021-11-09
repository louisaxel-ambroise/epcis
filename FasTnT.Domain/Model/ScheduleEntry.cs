namespace FasTnT.Domain.Model;

public class ScheduleEntry
{
    private readonly List<int> _values = new ();
    private readonly int _minValue, _maxValue;

    public int Min => _values.Min();

    public static ScheduleEntry Parse(string expression, int min, int max) => new (expression, min, max);
    public bool HasValue(int value) => _values.Contains(value);

    private ScheduleEntry(string expression, int min, int max)
    {
        _minValue = min;
        _maxValue = max;

        ParseExpression(expression);
    }

    private void ParseExpression(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            AddRange(_minValue, _maxValue);
        }
        else
        {
            foreach (var element in expression.Split(','))
            {
                ParseElement(element);
            }
        }
    }

    private void ParseElement(string element)
    {
        if (element.StartsWith("[") && element.EndsWith(']') && element.Contains('-'))
        {
            ParseRange(element);
        }
        else if (int.TryParse(element, out int value))
        {
            AddValue(value);
        }
        else
        {
            throw new ArgumentException($"Invalid value: {element}");
        }
    }

    private void ParseRange(string element)
    {
        var rangeParts = element[1..^1].Split('-');

        if (rangeParts.Length != 2)
        {
            throw new ArgumentException($"Invalid value: {element}");
        }
        if (int.TryParse(rangeParts[0], out int min) && int.TryParse(rangeParts[1], out int max))
        {
            AddRange(min, max);
        }
        else
        {
            throw new ArgumentException($"Invalid value: {element}");
        }
    }

    private void AddRange(int minValue, int maxValue)
    {
        if (minValue > maxValue || minValue < _minValue || maxValue > _maxValue)
        {
            throw new ArgumentException($"Invalid range value: [{minValue}-{maxValue}]");
        }

        _values.AddRange(Enumerable.Range(minValue, (maxValue - minValue + 1)));
    }

    private void AddValue(int value)
    {
        if (value < _minValue || value > _maxValue)
        {
            throw new ArgumentException($"Invalid value: {value}");
        }

        _values.Add(value);
    }
}
