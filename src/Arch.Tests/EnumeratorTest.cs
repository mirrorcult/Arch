using System.Runtime.InteropServices;
using Arch.Core;
using Arch.Core.Utils;
using static NUnit.Framework.Assert;

namespace Arch.Tests;

public partial class EnumeratorTest
{
    private static readonly ComponentType[] _group = { typeof(Transform), typeof(Rotation) };
    private static readonly ComponentType[] _otherGroup = { typeof(Transform), typeof(Rotation), typeof(Ai) };
    private readonly QueryDescription _description = new() { All = _group };

    private World _world;

    [OneTimeSetUp]
    public void Setup()
    {
        _world = World.Create();
        _world.Reserve(_group, 10000);
        _world.Reserve(_otherGroup, 10000);

        for (var index = 0; index < 10000; index++)
        {
            _world.Create(_group);
        }

        for (var index = 0; index < 10000; index++)
        {
            _world.Create(_otherGroup);
        }
    }

    [Test]
    public void WorldArchetypeEnumeration()
    {

        var bitset = new BitSet();
        bitset.SetBit(1);
        bitset.SetBit(5);

        Span<uint> stack = stackalloc uint[100];
        bitset.AsSpan(stack);

        var stackSet = new SpanBitSet(stack);
        //stackSet.SetBit(0);
        //stackSet.ClearBit(1);

        var counter = 0;
        foreach (ref var archetype in _world)
        {
            counter++;
        }

        That(counter, Is.EqualTo(2));
    }

    [Test]
    public void ArchetypeChunkEnumeration()
    {
        var counter = 0;
        var archetype = _world.Archetypes[0];
        foreach (ref var chunk in archetype)
        {
            counter++;
        }

        That(counter, Is.EqualTo((int)Math.Ceiling((float)10000 / archetype.CalculateEntitiesPerChunk(_group))));
    }

    [Test]
    public void QueryArchetypeEnumeration()
    {
        var counter = 0;
        var query = _world.Query(in _description);
        foreach (var archetype in query.GetArchetypeIterator())
        {
            counter++;
        }

        That(counter, Is.EqualTo(2));
    }

    [Test]
    public void QueryChunkEnumeration()
    {
        var counter = 0;
        var query = _world.Query(in _description);
        foreach (ref var chunk in query)
        {
            counter++;
        }

        var archetype1ChunkCount = _world.Archetypes[0].Size;
        var archetype2ChunkCount = _world.Archetypes[1].Size;
        That(counter, Is.EqualTo(archetype1ChunkCount + archetype2ChunkCount));
    }
}
