/*
 * [The "BSD license"]
 *  Copyright (c) 2013 Terence Parr
 *  Copyright (c) 2013 Sam Harwell
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *  1. Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The name of the author may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 *  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 *  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 *  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 *  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 *  THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Sharpen;

namespace Antlr4.Runtime
{
    /// <summary>
    /// An
    /// <see cref="IIntStream">IIntStream</see>
    /// whose symbols are
    /// <see cref="IToken">IToken</see>
    /// instances.
    /// </summary>
    public interface ITokenStream : IIntStream
    {
        /// <summary>
        /// Get the
        /// <see cref="IToken">IToken</see>
        /// instance associated with the value returned by
        /// <see cref="IIntStream.La(int)">LA(k)</see>
        /// . This method has the same pre- and post-conditions as
        /// <see cref="IIntStream.La(int)">IIntStream.La(int)</see>
        /// . In addition, when the preconditions of this method
        /// are met, the return value is non-null and the value of
        /// <code>LT(k).getType()==LA(k)</code>
        /// .
        /// </summary>
        /// <seealso cref="IIntStream.La(int)">IIntStream.La(int)</seealso>
        [return: NotNull]
        IToken Lt(int k);

        /// <summary>
        /// Gets the
        /// <see cref="IToken">IToken</see>
        /// at the specified
        /// <code>index</code>
        /// in the stream. When
        /// the preconditions of this method are met, the return value is non-null.
        /// <p/>
        /// The preconditions for this method are the same as the preconditions of
        /// <see cref="IIntStream.Seek(int)">IIntStream.Seek(int)</see>
        /// . If the behavior of
        /// <code>seek(index)</code>
        /// is
        /// unspecified for the current state and given
        /// <code>index</code>
        /// , then the
        /// behavior of this method is also unspecified.
        /// <p/>
        /// The symbol referred to by
        /// <code>index</code>
        /// differs from
        /// <code>seek()</code>
        /// only
        /// in the case of filtering streams where
        /// <code>index</code>
        /// lies before the end
        /// of the stream. Unlike
        /// <code>seek()</code>
        /// , this method does not adjust
        /// <code>index</code>
        /// to point to a non-ignored symbol.
        /// </summary>
        /// <exception cref="System.ArgumentException">if {code index} is less than 0</exception>
        /// <exception cref="System.NotSupportedException">
        /// if the stream does not support
        /// retrieving the token at the specified index
        /// </exception>
        [return: NotNull]
        IToken Get(int i);

        /// <summary>
        /// Gets the underlying
        /// <see cref="ITokenSource">ITokenSource</see>
        /// which provides tokens for this
        /// stream.
        /// </summary>
        ITokenSource TokenSource
        {
            get;
        }

        /// <summary>
        /// Return the text of all tokens within the specified
        /// <code>interval</code>
        /// . This
        /// method behaves like the following code (including potential exceptions
        /// for violating preconditions of
        /// <see cref="Get(int)">Get(int)</see>
        /// , but may be optimized by the
        /// specific implementation.
        /// <pre>
        /// TokenStream stream = ...;
        /// String text = "";
        /// for (int i = interval.a; i &lt;= interval.b; i++) {
        /// text += stream.get(i).getText();
        /// }
        /// </pre>
        /// </summary>
        /// <param name="interval">
        /// The interval of tokens within this stream to get text
        /// for.
        /// </param>
        /// <returns>
        /// The text of all tokens within the specified interval in this
        /// stream.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// if
        /// <code>interval</code>
        /// is
        /// <code>null</code>
        /// </exception>
        [return: NotNull]
        string GetText(Interval interval);

        /// <summary>Return the text of all tokens in the stream.</summary>
        /// <remarks>
        /// Return the text of all tokens in the stream. This method behaves like the
        /// following code, including potential exceptions from the calls to
        /// <see cref="IIntStream.Size()">IIntStream.Size()</see>
        /// and
        /// <see cref="GetText(Antlr4.Runtime.Misc.Interval)">GetText(Antlr4.Runtime.Misc.Interval)
        ///     </see>
        /// , but may be
        /// optimized by the specific implementation.
        /// <pre>
        /// TokenStream stream = ...;
        /// String text = stream.getText(new Interval(0, stream.size()));
        /// </pre>
        /// </remarks>
        /// <returns>The text of all tokens in the stream.</returns>
        [return: NotNull]
        string GetText();

        /// <summary>
        /// Return the text of all tokens in the source interval of the specified
        /// context.
        /// </summary>
        /// <remarks>
        /// Return the text of all tokens in the source interval of the specified
        /// context. This method behaves like the following code, including potential
        /// exceptions from the call to
        /// <see cref="GetText(Antlr4.Runtime.Misc.Interval)">GetText(Antlr4.Runtime.Misc.Interval)
        ///     </see>
        /// , but may be
        /// optimized by the specific implementation.
        /// <p/>
        /// If
        /// <code>ctx.getSourceInterval()</code>
        /// does not return a valid interval of
        /// tokens provided by this stream, the behavior is unspecified.
        /// <pre>
        /// TokenStream stream = ...;
        /// String text = stream.getText(ctx.getSourceInterval());
        /// </pre>
        /// </remarks>
        /// <param name="ctx">
        /// The context providing the source interval of tokens to get
        /// text for.
        /// </param>
        /// <returns>
        /// The text of all tokens within the source interval of
        /// <code>ctx</code>
        /// .
        /// </returns>
        [return: NotNull]
        string GetText(RuleContext ctx);

        /// <summary>
        /// Return the text of all tokens in this stream between
        /// <code>start</code>
        /// and
        /// <code>stop</code>
        /// (inclusive).
        /// <p/>
        /// If the specified
        /// <code>start</code>
        /// or
        /// <code>stop</code>
        /// token was not provided by
        /// this stream, or if the
        /// <code>stop</code>
        /// occurred before the
        /// <code>start</code>
        /// token, the behavior is unspecified.
        /// <p/>
        /// For streams which ensure that the
        /// <see cref="IToken.TokenIndex()">IToken.TokenIndex()</see>
        /// method is
        /// accurate for all of its provided tokens, this method behaves like the
        /// following code. Other streams may implement this method in other ways
        /// provided the behavior is consistent with this at a high level.
        /// <pre>
        /// TokenStream stream = ...;
        /// String text = "";
        /// for (int i = start.getTokenIndex(); i &lt;= stop.getTokenIndex(); i++) {
        /// text += stream.get(i).getText();
        /// }
        /// </pre>
        /// </summary>
        /// <param name="start">The first token in the interval to get text for.</param>
        /// <param name="stop">The last token in the interval to get text for (inclusive).</param>
        /// <returns>
        /// The text of all tokens lying between the specified
        /// <code>start</code>
        /// and
        /// <code>stop</code>
        /// tokens.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// if this stream does not support
        /// this method for the specified tokens
        /// </exception>
        [return: NotNull]
        string GetText(IToken start, IToken stop);
    }
}
