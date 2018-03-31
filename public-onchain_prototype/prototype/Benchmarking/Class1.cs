using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace MyBenchmarks
{
	[ClrJob(isBaseline: true), CoreJob, MonoJob]
	[RPlotExporter, RankColumn]
	public class Md5Bench
	{
		private MD5 md5 = MD5.Create();
		private byte[] data;

		[Params(1000, 10000)]
		public int N;

		[GlobalSetup]
		public void Setup()
		{
			data = new byte[N];
			new Random(42).NextBytes(data);
		}

		[Benchmark]
		public byte[] Md5() => md5.ComputeHash(data);
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<Md5Bench>();
		}
	}
}
