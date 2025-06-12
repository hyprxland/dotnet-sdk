bash := if os_family() == 'windows' {
    'C:\\PROGRA~1\Git\\usr\\bin\\bash.exe'
} else {
    '/usr/bin/env bash'
}


setup:
    #!{{ bash }}
    dotnet new install xunit.v3.templates


tpl cmd='add' :
    #!{{ bash }}
    echo "Running {{ cmd }} command"
    case "{{ cmd }}" in

        install|add|i|a|r)
            dotnet new install ./tpl/hxlib --force
            dotnet new install ./tpl/hxtest --force
            ;;
      
        uninstall|remove|r|u)
            dotnet new uninstall ./tpl/hxlib
            dotnet new uninstall ./tpl/hxtest
            ;;
        *)
            echo "Invalid command {{ cmd }}"
            ;;
    esac