Add this to your ```manifest.json``` before importing as package:
```
"scopedRegistries": [
    ...
    {
        "name": "package.openupm.com",
        "url": "https://package.openupm.com",
        "scopes": [
            "com.dbrizov.naughtyattributes",
            "com.github.siccity.xnode"
        ]
    },
    ...
]
```

