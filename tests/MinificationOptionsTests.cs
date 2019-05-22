#region Copyright (c) 2019 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace CSharpMinifier.Tests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    public class MinificationOptionsTests
    {
        [Test]
        public void Default()
        {
            Assert.That(MinificationOptions.Default, Is.Not.Null);
            Assert.That(MinificationOptions.Default.CommentFilter, Is.Null);
        }

        [Test]
        public void SetCommentFilterToFunction()
        {
            var filter = new Func<Token, string, bool>(delegate { throw new NotImplementedException(); });
            var options = MinificationOptions.Default.WithCommentFilter(filter);
            Assert.That(options, Is.Not.SameAs(MinificationOptions.Default));
            Assert.That(options.CommentFilter, Is.SameAs(filter));
        }

        [Test]
        public void ResetCommentFilterToNull()
        {
            var filter = new Func<Token, string, bool>(delegate { throw new NotImplementedException(); });
            var options = MinificationOptions.Default.WithCommentFilter(filter).WithCommentFilter(null);
            Assert.That(options.CommentFilter, Is.Null);
            Assert.That(options, Is.SameAs(MinificationOptions.Default));
        }

        [Test]
        public void SetCommentFilterToSame()
        {
            var options = MinificationOptions.Default.WithCommentFilter(null);
            Assert.That(options, Is.SameAs(MinificationOptions.Default));
            var filter = new Func<Token, string, bool>(delegate { throw new NotImplementedException(); });

            var options1 = options.WithCommentFilter(filter);
            Assert.That(options1, Is.Not.SameAs(options));

            var options2 = options1.WithCommentFilter(filter);
            Assert.That(options2, Is.SameAs(options1));
        }

        [Test]
        public void CommentMatchingWithNullPattern()
        {
            var e = Assert.Throws<ArgumentNullException>(() =>
                MinificationOptions.Default.WithCommentMatching(null));
            Assert.That(e.ParamName, Is.EqualTo("pattern"));

            e = Assert.Throws<ArgumentNullException>(() =>
                MinificationOptions.Default.WithCommentMatching(null, RegexOptions.None));
            Assert.That(e.ParamName, Is.EqualTo("pattern"));
        }

        [TestCase(@"^//"     , "foo"      , false)]
        [TestCase(@"^//"     , "/* foo */", false)]
        [TestCase(@"^//"     , "// foo"   , true )]
        [TestCase(@"^///[^/]", "// foo"   , false)]
        [TestCase(@"^///[^/]", "/// foo"  , true )]
        [TestCase(@"^///[^/]", "//// foo" , false)]
        public void CommentMatching(string pattern, string source, bool match)
        {
            var options = MinificationOptions.Default.WithCommentMatching(pattern);
            var token = Scanner.Scan(source).Single();
            Assert.That(options.CommentFilter(token, source), Is.EqualTo(match));
        }
    }
}