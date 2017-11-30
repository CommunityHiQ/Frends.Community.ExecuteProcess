- [Frends.Community.ExecuteProcess](#frendscommunityexecuteprocess)
  - [Contributing](#contributing)
  - [Documentation](#documentation)
    - [Input](#input)
    - [Result](#result)
  - [License](#license)


# Frends.Community.ExecuteProcess
FRENDS Task to execute extrenal scripts or processes. Executing with result takes timeout value and throws error if command is not completed within the given time. Executing without result simply fires off the command and continues.

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### Input

| Property				|  Type   | Description								| Example                     |
|-----------------------|---------|-----------------------------------------|-----------------------------|
| ScriptPath		| string	| Path to script | `%windir%\system32\cmd.exe` |
| Arguments			| array<string,string> 	| Command and command value	| `/C`, `echo testi >> c:\test.txt` |
| WaitForResponse	| bool	| Wait for process response	| `true` |
| Timeout Seconds	| int	| Timeout for process response in full seconds	| `10` |

### Result

| Property      | Type     | Description                      |
|---------------|----------|----------------------------------|
| Result        | string   | Command result	when waiting fo result |
| Status        | bool   | Status of executed command. Always true when returning a result. Returns true if command was started successfully when not waiting for a result.	|

## License

This project is licensed under the MIT License - see the LICENSE file for details
