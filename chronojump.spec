Name:           chronojump
Version:        0.8.9.5
Release:        2%{?dist}
Summary:        A measurement, management and statistics sport testing tool

Group:          Applications/Engineering
License:        GPLv2+
URL:            http://chronojump.org
Source0:        http://ftp.gnome.org/pub/GNOME/sources/chronojump/0.8/%{name}-%{version}.tar.gz
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)

BuildRequires:  mono-core pkgconfig mono-data-sqlite gtk-sharp2 gtk-sharp2-devel desktop-file-utils

%description
ChronoJump is an open hardware, free software, multiplatform complete system
for measurement, management and statistics of sport short-time tests.

Chronojump uses a contact platform and/or photocells, 
and also a chronometer printed circuit designed ad-hoc in
order to obtain precise and trustworthy measurements.

Chronojump is used by trainers, teachers and students.



%prep
%setup -q


%build
%configure
make %{?_smp_mflags}


%install
rm -rf %{buildroot}
make install DESTDIR=%{buildroot}

# this file should be in the standard dir
rm %{buildroot}/%{_datadir}/doc/chronojump/chronojump_manual_es.pdf

# removing non used files:
rm %{buildroot}/%{_libdir}/chronojump/libchronopic.a
rm %{buildroot}/%{_libdir}/chronojump/libchronopic.la

desktop-file-install --dir=%{buildroot}%{_datadir}/applications/   %{buildroot}%{_datadir}/applications/chronojump.desktop

%find_lang %{name}

%clean
rm -rf %{buildroot}

%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%files -f %{name}.lang
%defattr(-,root,root,-)
%{_bindir}/chronojump
%{_bindir}/chronojump_mini
%{_bindir}/test-accuracy
%{_bindir}/test-jumps
%{_bindir}/test-stream
%dir %{_libdir}/chronojump
%{_libdir}/chronojump/*
%dir %{_datadir}/chronojump
%{_datadir}/chronojump/*
%{_datadir}/icons/hicolor/48x48/apps/chronojump.png
%{_datadir}/applications/chronojump.desktop

%doc README COPYING AUTHORS manual/chronojump_manual_es.pdf


%changelog

* Wed Aug 26 2009 <ismael@olea.org> 0.8.9.5-2
- minor spec typos
- Use %%find_lang.
- added ldconfig invocation
- removed libchronopic.la

* Tue Aug 25 2009 <ismael@olea.org> 0.8.9.5-1
- first release
