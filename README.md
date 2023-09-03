# appins
Just offlineins, but shorter text
# Commands
CMD-line help:
- `appins help`: help
- `appins upload <your-public-repo-on-github> <executable-path>`: Generate an ID to download, `<executable-path>` is a relative path (e.g. `C:\a.exe` -> `a.exe`) or `""` in command line (e.g. `appins upload ... ""`)
- `appins program <id>`: Download a program (require admin bcz it stores in `"C:\Program Files"`)
- `appins local <id>`: Download a program, store in `AppData`
- `appins current <id>`: Download a program, store in current dir (may require admin)
- `appins ids <id>`: Browse IDs that larger than `<id>` (not all)
- (default) `appins ids 0`: Browse IDs that larger than 0 (not all)
- `appins users <id>`: Browse user IDs that larger than `<id>` (not all)
- (default) `appins users 0`: Browse user IDs that larger than 0 (not all)
- `appins user <user>`: Browse IDs from `<user>`
IDE help:
`help`: help
`upload <your-public-repo-on-github> <executable-path>`: Generate an ID to download, `<executable-path>` is a relative path (e.g. `C:\a.exe` -> `a.exe`) or `""` in command line (e.g. `appins upload ... ""`)
`program <id>`: Download a program (require admin bcz it stores in `"C:\Program Files"`)
`local <id>`: Download a program, store in `AppData`
`ids <id>`: Browse IDs that larger than `<id>` (not all)
(default) `ids 0`: Browse IDs that larger than 0 (not all)
`users <id>`: Browse user IDs that larger than `<id>` (not all)
(default) `users 0`: Browse user IDs that larger than 0 (not all)
`user <user>`: Browse IDs from `<user>`
# Notes (for uploaders)
- You must know about GitHub (`https://github.com`) to upload
- Use full URL (e.g. `https://github.com/.../...`, not `github.com/.../...` and not `.../...`)