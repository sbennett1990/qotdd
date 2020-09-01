# Build libs for Mono on OpenBSD

LIB=	libcmdline

SRCS=	CommandLineProcessor.cs Option.cs OptionEventArgs.cs
SRCS+=	Properties/AssemblyInfo.cs

DOCFILE=	${LIB}.doc

SRCDIR= ${LIB}
BINDIR=	bin

# Which C# language version to compile with
LANG=	7.3

.include "mono.lib.mk"
