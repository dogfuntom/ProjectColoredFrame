// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Drawing;

namespace ProjectColoredFrame
{
	/// <summary>
	/// DTO struct to be used in <see cref="ProjectColoredFrameOptionGrid"/> only.
	/// </summary>
	public struct CustomMapping : ICustomMapping
	{
		public string Wildcard { get; set; }
		public Color Color { get; set; }
	}

	/// <summary>
	/// Read-only interface for <see cref="CustomMapping"/>.
	/// This iterface exists just to emphasize that it should be treated as immutable.
	/// </summary>
	public interface ICustomMapping
	{
		Color Color { get; }
		string Wildcard { get; }
	}
}