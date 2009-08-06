class Compiler
	def self.compile(attributes)
		version = attributes.fetch(:clrversion, 'v3.5')
		compile_target = attributes.fetch(:compile_target, 'debug')
		solution_file = attributes[:solution_file]

		framework_dir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', 'v3.5')
		msbuild_file = File.join(framework_dir, 'MSBuild.exe')

		sh "#{msbuild_file} #{solution_file} /property:Configuration=#{compile_target} /t:Rebuild"
	end
end

class Tester
	def initialize(items)
		@source_dir = items.fetch(:source_dir, 'src')
		@gallio_dir = items.fetch(:gallio_dir, 'thirdparty/tools/gallio')
		#TODO:  Rethink default name
		@test_results_dir = items.fetch(:test_results_dir, 'artifacts')
		@compile_target = items.fetch(:compile_target, 'debug')
		@show_report = items.fetch(:show_report, false)
		@report_type = items.fetch(:report_type, 'XML')
	end
	
	def run(assemblies)
		assemblies.each do |assembly|
			sh build_command_for(assembly)
		end
	end
	
	def build_command_for(assembly)
		file = File.expand_path("#{@source_dir}/#{assembly}/bin/#{@compile_target}/#{assembly}.dll")
		"#{@gallio_dir}/Gallio.Echo.exe #{file} /rt:#{@report_type} /rd:#{@test_results_dir} #{'/sr' if @show_report}"
	end
end
