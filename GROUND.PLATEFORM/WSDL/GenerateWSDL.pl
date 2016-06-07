my @files = get_files();

foreach my $file (@files)
{
	print "$file\n";
	process($file);
}

exit 0;


#========================================================================================

sub get_files()
{
	my @result;
	
	opendir(DIR, ".");
	my @FILES= readdir(DIR);
	foreach my $filename (@FILES)
	{
		if ($filename =~ /.*\.wsdl$/)
		{
			push (@result, $filename);
		}
		elsif ($filename =~ /.*\.xsd$/)
		{
			push (@result, $filename);
		}	
	}
	closedir DIR;
	return @result;
}

#========================================================================================

sub process()
{
	my $filein = shift;
	my $fileout = "$filein.tmp";
	
	open IN, "<$filein";
	open OUT, ">$fileout";
	
	while(<IN>)
	{
		if (/import (?:schemaLocation="" )?namespace="(.*?)"/)
		{
			my $ns = $1;
			my $sc = "";
			if ($ns =~ /http:\/\/(alstom.*)/)
			{
				$sc = $1;
								
				if ($sc =~ /\/$/)
				{
					$sc =~ s/\/$/.xsd/; # if namespace ends with slash, schema file has suffix ".xsd"
				}
				else
				{
					$sc =~ s/$/1.xsd/; 	# if namespace doesn't end with slash, schema file has suffix "1.xsd"
				}
				
				$sc =~ s/\//\./g;		# replace all remaining slashes with dots
				$sc =~ s/^/.\\/; 		# add ".\" prefix for local file
				
			}			
			elsif ($ns =~ /http:\/\/(schemas.microsoft.*)/)
			{
				$sc = $1;
								
				$sc =~ s/(\/)?$/.xsd/; 	# remove (optional) slash at the end of the namespace, append schema file with suffix ".xsd"
				
				$sc =~ s/\//\./g;		# replace all remaining slashes with dots
				$sc =~ s/^/.\\/; 		# add ".\" prefix for local file
				
			}
			elsif ($ns =~ /http:\/\/(schemas.datacontract.*)/)
			{
				$sc = $1;
								
				$sc =~ s/(.*)\///; 		# remove everything up to (and including) the last slash
				$sc =~ s/$/.xsd/;		# append schema file with suffix ".xsd"
				$sc =~ s/^/.\\/; 		# add ".\" prefix for local file
				
			}
			s/import (schemaLocation="" )?namespace=".*?"/import schemaLocation="$sc" namespace="$ns"/;
		}
		
		if (/address location="http:\/\/(.*?)\//)
		{
			s/address location="http:\/\/(.*?)\//address location="http:\/\/alstom-ground-pis-server\//;
		}
		print OUT "$_";
	}
	close IN;
	close OUT;
	
	rename $fileout, $filein;
	
	return @result;
}