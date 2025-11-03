var ajaxPageReq;
var ajaxListReq;

function createAutoComplete(service, keyAttribute, valueAttribute) {
	textBox = document.getElementById(valueAttribute);
	idTextBox = document.getElementById(keyAttribute);
	var currentElement, a, b;
	textBox
			.addEventListener(
					"input",
					function(e) {
						var i;
						closeAllLists();
						if (!this.value) {
							return false;
						}
						// if(searchText.length<3) { return false; }
						currentElement = -1;
						document.getElementById(keyAttribute).value = '';
						a = textBox;
						try {
							ajaxListReq.abort();
						} catch (e) {
						}
						ajaxListReq = new XMLHttpRequest();
						ajaxListReq.onreadystatechange = function() {
							if (this.readyState == 4 && this.status == 200) {
								var list = JSON.parse(this.responseText);
								var i, hidden;
								closeAllLists();
								for (i = 0; i < list.length; i++) {
									b = document.createElement("DIV");
									hidden = list[i][keyAttribute] + ','
											+ list[i][valueAttribute];
									b.innerHTML = list[i][valueAttribute]
											+ "<input type='hidden' value='"
											+ hidden + "'/>";
									b
											.addEventListener(
													"click",
													function(e) {
														var keyValue = this
																.getElementsByTagName('input')[0].value;
														var keyAndValue = keyValue
																.split(',');
														document
																.getElementById(keyAttribute).value = keyAndValue[0];
														document
																.getElementById(valueAttribute).value = keyAndValue[1];
														closeAllLists();
													});
									a.appendChild(b);
								}
							}
						};
						ajaxListReq.open("GET", service + '/' + this.value,
								true);
						ajaxListReq.send();
					});
	textBox.addEventListener("keydown", function(e) {
		var x = document.getElementById(this.id + "autocomplete-list");
		if (x)
			x = x.getElementsByTagName("div");
		if (e.keyCode == 40) {
			currentElement++;
			addActive(x);
		} else if (e.keyCode == 38) {
			currentElement--;
			addActive(x);
		} else if (e.keyCode == 13) {
			e.preventDefault();
			if (currentElement > -1) {
				if (x)
					x[currentElement].click();
			}
		}
	});

	function addActive(x) {

		if (!x)
			return false;

		removeActive(x);
		if (currentElement >= x.length)
			currentElement = 0;
		if (currentElement < 0)
			currentElement = (x.length - 1);

		x[currentElement].classList.add("autocomplete-active");
	}

	function removeActive(x) {

		for (var i = 0; i < x.length; i++) {
			x[i].classList.remove("autocomplete-active");
		}
	}
	function closeAllLists() {
		/*
		 * close all autocomplete lists in the document, except the one passed
		 * as an argument:
		 */
		var x = document.getElementsByClassName("autocomplete-items");
		for (var i = 0; i < x.length; i++) {
			x[i].parentNode.removeChild(x[i]);
		}
		a = document.createElement("DIV");
		a.setAttribute("id", valueAttribute + "autocomplete-list");
		a.setAttribute("class", "autocomplete-items");
		document.getElementById(valueAttribute).parentNode.appendChild(a);
	}
	document.addEventListener("click", function(e) {
		closeAllLists();
	});
}

function createAutoComplete3(service, keyAttribute1, keyAttribute2, keyAttribute3, valueAttribute) {
    textBox = document.getElementById(valueAttribute);
    idTextBox1 = document.getElementById(keyAttribute1);
    idTextBox2 = document.getElementById(keyAttribute2);
    idTextBox3 = document.getElementById(keyAttribute3);
    var currentElement, a, b;
    textBox
			.addEventListener(
					"input",
					function (e) {
					    var i;
					    closeAllLists();
					    if (!this.value) {
					        return false;
					    }
					    // if(searchText.length<3) { return false; }
					    currentElement = -1;
					    document.getElementById(keyAttribute1).value = '';
					    document.getElementById(keyAttribute2).value = '';
					    document.getElementById(keyAttribute3).value = '';
					    a = textBox;
					    try {
					        ajaxListReq.abort();
					    } catch (e) {
					    }
					    ajaxListReq = new XMLHttpRequest();
					    ajaxListReq.onreadystatechange = function () {
					        if (this.readyState == 4 && this.status == 200) {
					            var list = JSON.parse(this.responseText);
					            var i, hidden;
					            closeAllLists();
					            for (i = 0; i < list.length; i++) {
					                b = document.createElement("DIV");
					                hidden = list[i][keyAttribute1] + ',' + list[i][keyAttribute2]
                                             + ',' + list[i][keyAttribute3] + ',' + list[i][valueAttribute];
					                b.innerHTML = list[i][valueAttribute]
											+ "<input type='hidden' value='"
											+ hidden + "'/>";
					                b
											.addEventListener(
													"click",
													function (e) {
													    var keyValue = this
																.getElementsByTagName('input')[0].value;
													    var keyAndValue = keyValue.split(',');
													    document.getElementById(keyAttribute1).value = keyAndValue[0];
													    document.getElementById(keyAttribute2).value = keyAndValue[1];
													    document.getElementById(keyAttribute3).value = keyAndValue[2];
													    document.getElementById(valueAttribute).value = keyAndValue[3];
													    closeAllLists();
													});
					                a.appendChild(b);
					            }
					        }
					    };
					    ajaxListReq.open("GET", service + '/' + this.value,
								true);
					    ajaxListReq.send();
					});
    textBox.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x)
            x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            currentElement++;
            addActive(x);
        } else if (e.keyCode == 38) {
            currentElement--;
            addActive(x);
        } else if (e.keyCode == 13) {
            e.preventDefault();
            if (currentElement > -1) {
                if (x)
                    x[currentElement].click();
            }
        }
    });

    function addActive(x) {

        if (!x)
            return false;

        removeActive(x);
        if (currentElement >= x.length)
            currentElement = 0;
        if (currentElement < 0)
            currentElement = (x.length - 1);

        x[currentElement].classList.add("autocomplete-active");
    }

    function removeActive(x) {

        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists() {
        /*
		 * close all autocomplete lists in the document, except the one passed
		 * as an argument:
		 */
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            x[i].parentNode.removeChild(x[i]);
        }
        a = document.createElement("DIV");
        a.setAttribute("id", valueAttribute + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        document.getElementById(valueAttribute).parentNode.appendChild(a);
    }
    document.addEventListener("click", function (e) {
        closeAllLists();
    });
}


function ajaxGet(serviceName, onSuccess) {
	try {
		ajaxPageReq.abort();
	} catch (e) {
	}
	ajaxPageReq = new XMLHttpRequest();
	ajaxPageReq.onreadystatechange = function() {
		if (this.readyState == 1) {
			document.getElementById("content").innerHTML = "Loading....";
		} else if (this.readyState == 4 && this.status == 200) {
			document.getElementById("content").innerHTML = this.responseText;
			if (onSuccess != null) {
				onSuccess();
			}
		}
	};
	ajaxPageReq.open("GET", serviceName, true);
	ajaxPageReq.send();
}

function showHome() {
	ajaxGet('../App/Home');
}

