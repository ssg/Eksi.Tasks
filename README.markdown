Eksi.Tasks
==========

Eksi.Tasks currently provides NAnt task wrappers around Douglas Crockford's JSLint and Yahoo's YUI Compressor library's !JavaScript and CSS compressors.

In order to preserve compatibility with jsmin/cssmin the same syntax is used. Basic syntax is:

```xml
    <jslint failonerrors="true" config="/*jslint whitespace:true */">
      <fileset>
        <include name="js/*.js" />
      </fileset>
    </jslint>

    <jsmin todir="dest_path" suffix="-min">
      <fileset>
        <include name="js/*.js" />
      </fileset>
    </jsmin>

    <cssmin todir="dest_path" suffix="-min">
      <fileset>
        <include name="css/*.css" />
      </fileset>
    </cssmin>
```

Copyright
=========

Copyright (c) 2010 Ekşi Teknoloji Ltd. (www.eksiteknoloji.com)
Licensed under MIT License -- see license.txt for details

We are willing to release anything that we would have liked to find 
available in the wild as open source.

As you can see this project starts only with a JSLint task because there
was none available for NAnt. More to come :) 

Contact:

Sedat Kapanoglu
sedat@eksiteknoloji.com
