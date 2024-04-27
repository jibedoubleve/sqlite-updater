using System.Collections;

namespace System.SQLite.Updater.Core;

internal class ScriptCollection : IEnumerable<string>
{
    #region Fields

    private readonly IDictionary<Version, string> _resources;

    #endregion

    #region Constructors

    public ScriptCollection(IDictionary<Version, string> resources)
    {
        if (resources is null) throw new ArgumentNullException(nameof(resources));
        _resources = resources;
    }

    #endregion

    #region Methods

    public IEnumerable<string> After(Version ver) { return from version in _resources.Keys where version > ver select _resources[version]; }

    public IEnumerator<string> GetEnumerator() { return _resources.Values.GetEnumerator(); }

    public Version MaxVersion() { return _resources.Keys.Max() ?? new Version(); }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    #endregion

    #region Indexers

    public string this[Version version] => _resources.TryGetValue(version, out var value) ? value : string.Empty;

    #endregion Indexers
}