# Build qotdd for Mono on OpenBSD

PROG=	qotdd

SRCS=	qotdd.cs listener.cs reader.cs
SRCS+=	properties/assemblyinfo.cs

MAIN=	Program

SRCDIR=	src
BINDIR=	bin

LANG=	7.3

CSSYMBOL=	NET45

.include "mono.prog.mk"
