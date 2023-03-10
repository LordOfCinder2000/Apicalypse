using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using Apicalypse.NamingPolicies;
using Apicalypse.Configuration;
using System.Linq;
using Apicalypse.Extensions;
using System.ComponentModel;
using NUnit.Framework;
using Apicalypse.Attributes;

namespace Apicalypse.Test.Builders;
public enum Test
{
    a, b
}

public class Genre
{
    public string Name { get; set; }
}

public class Game
{
    //public string Name { get; set; }
    //public string Slug { get; set; }
    //public uint Follows;
    //public double Score { get; set; }
    //public DateTime ReleaseDate { get; set; }
    //public Test Test { get; set; }
    //public List<int> Tags { get; set; }

    [Include]
    [DisplayName("Genres")]
    public IReadOnlyList<Genre> Categories { get; set; }
}

public class QueryTest
{
    [SetUp]
    public void Setup()
    {

    }
    class Test
    {
        public int[] Tags { get; set; }
    }

    //[Test]
    //public async Task TestSelect()
    //{
    //    var builder = new QueryBuilder<Game>(new QueryBuilderOptions { NamingPolicy = NamingPolicy.SnakeCase});

    //    Expression<Func<Game, object>> predicate = g => g.Name;
    //    Expression<Func<Game, object>> predicate2 = g => g.Follows;

    //    var source = new int[] { 1, 2, 3 };
    //    var source2 = new int[] { 2, 3, 5 };
    //    var target = new int[] { 5 };


    //    var test = new Test();
    //    test.Tags = source;

    //    var test2 = new Test();
    //    test2.Tags = source2;

    //    var tests = new List<Test> { test, test2 };

    //    var rs = tests.Where(t => target.Any(i => t.Tags.Contains(i))).ToList();

    //    //var str0 = new QueryBuilder<Game>().Select<Game>().Where(g => g.Tags.Contains(9)).Build();
    //    var number = new int[] { 1 };
    //    var temp = 1;
    //    var str = new QueryBuilder<Game>().Select<Game>().Where(g => g.Follows == 1 && g.Follows < 1).Build();

    //    var b = builder
    //        .Select(o => new
    //        { // the list of fields to gather
    //            o.Name,
    //            o.Slug
    //        })
    //        .Where( // conditions
    //            o => o.Follows > 3
    //            && o.Follows < 10
    //        )
    //        .OrderByDescending(   // Descending sort orer
    //            o => o.ReleaseDate
    //        )
    //        .Take(8) // limit to 8 results
    //        .Skip(0).Build(); // gather results after the third one
    //    var body = new StringContent(b);
    //    var httpClient = new HttpClient();
    //    var httpRequestMessage = new HttpRequestMessage
    //    {
    //        Method = HttpMethod.Post,
    //        RequestUri = new Uri("https://api.igdb.com/v4/games"),
    //        Headers = {
    //            { "Authorization", "Bearer gxevr0oqg3blg70ajxxo3xzyw7byi4" },
    //            { "Client-ID", "4fkki7x67mrp71i49qcau3o2r47yi1" },
    //        },
    //        Content = body
    //    };

    //    var response = await httpClient.SendAsync(httpRequestMessage);
    //    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    //}
}