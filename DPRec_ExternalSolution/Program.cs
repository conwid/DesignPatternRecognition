using DPRec_Lib.CSharpImplementation;
using DPRec_Lib.Logging;
using DPRec_Lib.Profiling;
using DPRec_Lib.Recognition.ChainOfResponbility;
using DPRec_Lib.Recognition.Composite;
using DPRec_Lib.Recognition.Decorator;
using DPRec_Lib.Recognition.FactoryMethod;
using DPRec_Lib.Recognition.Mediator;
using DPRec_Lib.Recognition.Model;
using DPRec_Lib.Recognition.Proxy;
using DPRec_Lib.Recognition.Singleton;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_ExternalSolution
{
	class Program
	{
		static void Main( string[] args )
		{
			LoggerProvider lp = new LoggerProvider();


			var solution = @"EPPlus.sln";
			CSharpRecognitionContext ctx = new CSharpRecognitionContext( "EPPlus" );




			ProfilerService.Init( typeof( NullProfiler ), ( s, t ) => { } );

			using( ProfilerService.Current.Section( "Total" ) )
			{
				using( ProfilerService.Current.Section( "GetCompilations" ) )
				{
					ctx.Compilations = CSharpModelBuilder.GetRecognitionContextInputFromSolution( solution );
				}
				using( ProfilerService.Current.Section( "Init" ) )
				{
					ctx.Init( lp );
				}
				SingletonRecognizer srec = new SingletonRecognizer();
				srec.Context = ctx;

				CompositeRecognizer crec = new CompositeRecognizer();
				crec.Context = ctx;

				//ProxyRecognizer prec = new ProxyRecognizer();
				//prec.Context = ctx;

				ChainOfResponsiblityRecognizer correc = new ChainOfResponsiblityRecognizer();
				correc.Context = ctx;

				FactoryMethodRecognizer frec = new FactoryMethodRecognizer();
				frec.Context = ctx;

				DecoratorRecognizer drec = new DecoratorRecognizer();
				drec.Context = ctx;

				MediatorRecognizer mmrec = new MediatorRecognizer();
				mmrec.Context = ctx;

				Console.Clear();

				using( var fres = File.CreateText( "results.txt" ) )
				{

					foreach( var item in ctx.Types )
					{

						try
						{
							if( mmrec.IsInstance( item ) )
							{
								Console.WriteLine( "{0} is a mediator base", item.ToString() );
								fres.WriteLine( string.Format( "{0} is a singleton", item.ToString() ) );
							}
						}
						catch
						{ }

						try
						{
							if( srec.IsInstance( item ) )
							{
								Console.WriteLine( string.Format( "{0} is a singleton", item.ToString() ) );
								fres.WriteLine( string.Format( "{0} is a singleton", item.ToString() ) );
							}
						}
						catch( Exception )
						{ }


						try
						{
							if( crec.IsInstance( item ) )
							{
								Console.WriteLine( string.Format( "{0} is a composite", item.ToString() ) );
								fres.WriteLine( string.Format( "{0} is a composite", item.ToString() ) );
							}
						}
						catch( Exception )
						{

						}

						//if (prec.isinstance(item))
						//{
						//    console.writeline(string.format("{0} is a proxy", item.tostring()));
						//    fres.writeline(string.format("{0} is a proxy", item.tostring()));
						//}
						try
						{
							if( correc.IsInstance( item ) )
							{
								Console.WriteLine( string.Format( "{0} is a chain of responsiblity base", item.ToString() ) );
								fres.WriteLine( string.Format( "{0} is a chain of responsiblity base", item.ToString() ) );
							}

						}
						catch( Exception )
						{

						}

						try
						{

							if( drec.IsInstance( item ) )
							{
								Console.WriteLine( string.Format( "{0} is a decorator", item.ToString() ) );
								fres.WriteLine( string.Format( "{0} is a decorator", item.ToString() ) );
							}
						}
						catch( Exception )
						{

						}
					}



					foreach( var item in ctx.Types.SelectMany( t => t.Methods ) )
					{
						try
						{
							if( frec.IsInstance( item ) )
							{
								Console.WriteLine( string.Format( "{0} is a factory method", item.ToString() ) );
								fres.WriteLine( string.Format( "{0} is a factory method", item.ToString() ) );
							}
						}
						catch( Exception )
						{
						}
					}
				}
			}
			Console.ReadLine();
		}
	}
}
