require 'fileutils'
require 'build_utilities.rb'

COMPILE_TARGET = 'debug'
ARTIFACTS_DIR = 'artifacts'
SOLUTION_FILE = 'src/SineSignal.Ottoman.sln'
COMPANY = 'SineSignal, LLC'
COPYRIGHT = 'Copyright © SineSignal, LLC 2007-2009'
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs'

desc "[Default], compiles and runs unit tests"
task :default => [:compile, :test_unit]

def get_version
	return ENV['BUILD_NUMBER'].to_s unless ENV['BUILD_NUMBER'].nil?
	return "1.0.0.0"
end

desc "Update the version information for the build"
task :version do
	builder = AsmInfoBuilder.new get_version(),
							   :company => COMPANY,
                               :copyright => COPYRIGHT,
                               :allow_partially_trusted_callers => true
  buildNumber = builder.buildnumber
  puts "The build number is #{buildNumber}"
  builder.write COMMON_ASSEMBLY_INFO
end

desc "Prepares the working directory for a new build"
task :clean do
end

desc 'Compiles the Ottoman project'
task :compile => [:clean, :version] do
	#TODO:  Add a central location to set the value of our project name
	Compiler.compile :compile_target => COMPILE_TARGET, :solution_file => SOLUTION_FILE
end

desc 'Run all tests for the Ottoman project'
task :test_all => :compile do
	runner = Tester.new :compile_target => COMPILE_TARGET, :show_report => false
	#TODO:  Add a central location to set the value of our test dll
	runner.run "SineSignal.Ottoman.Tests"
end

desc 'Run unit tests for the Ottoman project'
task :test_unit => :compile do
	runner = Tester.new :compile_target => COMPILE_TARGET, :show_report => false, :filter_category => 'Unit'
	#TODO:  Add a central location to set the value of our test dll
	runner.run "SineSignal.Ottoman.Tests"
end

desc 'Run integration tests for the Ottoman project'
task :test_integration => :compile do
	runner = Tester.new :compile_target => COMPILE_TARGET, :show_report => false, :filter_category => 'Integration'
	#TODO:  Add a central location to set the value of our test dll
	runner.run "SineSignal.Ottoman.Tests"
end
