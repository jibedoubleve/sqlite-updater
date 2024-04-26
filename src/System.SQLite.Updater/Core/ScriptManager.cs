using System.Reflection;
using System.Text.RegularExpressions;

namespace System.SQLite.Updater.Core;

internal class ScriptManager
{
    #region Fields

    private readonly Assembly _asm;
    private readonly Regex _regex;

    #endregion

    #region Constructors

    public ScriptManager(Assembly asm, string pattern)
    {
        if (asm is null) throw new ArgumentNullException(nameof(asm));
        if (pattern is null) throw new ArgumentNullException(nameof(pattern));

        _regex = new Regex(pattern);
        _asm   = asm;
    }

    #endregion

    #region Methods

    public ScriptCollection GetScripts()
    {
        var dico = GetResources();
        var src  = new SortedDictionary<Version, string>();

        foreach (var item in dico)
        {
            var regex = new Regex(@"^.*?(\d{1,3}\.{0,1}\d{1,3}\.{0,1}\d{0,3}).*");
            var match = regex.Matches(item.Key);
            if (match.Count <= 0 || match[0].Groups.Count < 1) continue;

            var ver     = match[0].Groups[1].Value.Trim('.');
            var version = new Version(ver);
            src.Add(version, item.Value);
        }

        var ordered = src.OrderBy(x => x.Key);
        return new ScriptCollection(new Dictionary<Version, string>(ordered));
    }

    private string GetResource(string resourceName)
    {
        using var stream = _asm.GetManifestResourceStream(resourceName);
        if (stream == null) throw new ArgumentNullException(nameof(resourceName), $"'{nameof(resourceName)}' is null. Are you sure the resource '{resourceName}' exists in the assembly '{_asm.FullName}'");
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private IDictionary<string, string> GetResources()
    {
        var dico = new Dictionary<string, string>();

        foreach (var item in ListResources()) dico.Add(item, GetResource(item));

        return dico;
    }


    private IEnumerable<string> ListResources()
    {
        var result = from s in _asm.GetManifestResourceNames() where _regex.IsMatch(s) select s;
        return result;
    }

    #endregion
}