function openFile(data) {
  var link = this.document.createElement("a");

  link.download = data.fileName;
  link.href = data.url;

  this.document.body.appendChild(link);

  link.click();

  this.document.body.removeChild(link);
}
