require 'fileutils'
require 'build_utilities.rb'

COMPILE_TARGET = 'debug'

task :default => :build

task :build => :test

desc 'Compiles the Ottoman project'
task :compile do
	#TODO:  Add a central location to set the value of our project name
	Compiler.compile :compile_target => COMPILE_TARGET, :solution_file => 'src/SineSignal.Ottoman.sln'
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
