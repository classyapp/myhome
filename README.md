myhome
======

### Getting Started

1. update your hosts file so that `local.myhome.com` points to `127.0.0.1`
  * open Wordpad as Administrator
  * navigate to `c:\windows\system32\etc\drivers\hosts`
  * append to the file: `127.0.0.1      localhost`
* open `MyHome.sln`, if you get warnings about needing to change your host to `localhost`, you'll need to manually update your IIS Express settings to serve from `local.myhome.com` like so:
  * navigate to your `c:\Users\[YOUR_USERNAME]\Documents\IISExpress\config`
  * edit the file `applicationhost.config` (it may open automatically in VS but it's XML and may be edited in any text editor)
  * search for `<sites>`
  * find the child `<site>` element with a virtual directory pointing to this project
  *   replace the `<bindings>` element of that `<site>` element with the following:

      ```
      <bindings>
        <binding protocol="http" bindingInformation="*:8080:local.homelab.com" />
        <binding protocol="https" bindingInformation="*:44300:local.homelab.com" />
      </bindings>
      ```
* you should now be able to open `MyHome.sln`. **warning:** IISExpress doesn't like serving SSL content on anything except `localhost` so you might have to run VS as an Administrator (though this shouldn't pose too much of an issue for the most part since ssl is not required for local requests)

 
 
