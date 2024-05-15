function set_rendering_notice(content) {
    document.getElementById("rendering-notice").style.display = '';
    document.getElementById("rendering-notice").innerHTML = content + "<div class=\"loader\"></div> ";
}

function hide_rendering_notice() {
    document.getElementById("rendering-notice").style.display = 'none';
}

function load_json(path, callback) {
    var target = path + "?t=" + Math.round(Date.now()/1000);
    if (path.includes(".php")) {
        target = path;
    }
    fetch(target).then((req) => {
        req.json().then(parsed => {
            callback(parsed);
        })
    });
}

function load_components(callback) {
    // This loads components in order until done
    function load_components_iterative(remaining, callback, done) {
        var to_load = remaining.pop()
        fetch(to_load["path"] + "?t=" + Math.round(Date.now()/1000)).then((req) => {
            req.text().then(loaded => {
                done[to_load["name"]] = loaded;
                if (remaining.length == 0) {
                    callback(done);
                    return;
                }
                load_components_iterative(remaining, callback, done);
            })
        });
    }
    load_json("data/components.json", to_load => {
        load_components_iterative(to_load, callback, {});
    });
}

function load_data(data_url, callback) {
    load_json(data_url, data => {
        // This is the data that needs to be rendered! Do operations to render now
        callback(data);
    })
}

function render_section(section, components) {
    var component = components[section.type];
    if (component == undefined) {
        console.warn("found request for undefined component " + section.type + " - will return an empty string!");
        return "";
    }
    if (section.format != undefined) {
        // Configuration required! Do that now
        var keys = Object.keys(section.format);
        for (var i = 0; i < keys.length; i++) {
            while (component.includes("{" + keys[i] + "}")) {
                component = component.replace("{" + keys[i] + "}", section.format[keys[i]]);
            }
        }
    }
    // TODO: Do regex to find and replace any remaining keys
    return component;
}

function render_section_to_page(rendered, section) {
    if (document.getElementById(section.name) == null) {
        console.error("could not find element with name " + section.name + " for use as a section");
        return false;
    }
    var el = document.getElementById(section.name);
    el.outerHTML = rendered;
    el.id = section.name;
    return true;
}

function render(components, manifest) {
    rendered = 0;
    for (var i = 0; i < manifest.length; i++) {
        if (render_section_to_page(render_section(manifest[i], components), manifest[i])) {
            rendered += 1;
        }
    }
    return rendered;
}

function render_from_page_data(pageData, target) {
    document.getElementById(target).innerHTML = pageData.html;
    var oldRenderedCount = 0;
    while(true) {
        var renderCount = render(window.web3D.components, pageData.manifest);
        if (oldRenderedCount == renderCount) break;
        oldRenderedCount = renderCount;
    }
    hide_rendering_notice();
}

function render_page_from_id(pageId) {
    var evalElementsStart = document.getElementsByClassName('eval-on-page-change-start');
    for (var i = 0; i < evalElementsStart.length; i++) {
        eval(evalElementsStart[i].innerHTML);
    }
    var active = document.getElementsByClassName('active');
    for (var i = 0; i < active.length; i++) {
        active[i].classList.remove('active');
    }
    set_rendering_notice("rendering " + pageId);
    var pageData = window.web3D.loadedPages[pageId];
    render_from_page_data(pageData, "page")
    var evalElements = document.getElementsByClassName('eval-on-page-change-end');
    for (var i = 0; i < evalElements.length; i++) {
        eval(evalElements[i].innerHTML);
    }
    var navEl = document.getElementById('nav-' + pageId);
    if (navEl != undefined) {
        navEl.classList.add('active');
    }
}

function render_default() {
    var components = window.web3D.components;
    var manifest = window.web3D.default;
    render(components, manifest);
}

function load_pages_manifest(callback) {
    fetch('data/pages.json').then(req => {
        req.json().then(manifest => callback(manifest));
    });
}

function load_page(pageId, callback) {
    var pageRoot = "data/pages/" + pageId;
    fetch(pageRoot + "/data.json?t" + Math.round(Date.now()/1000)).then(req => {
        req.json().then(pageManifest => {
            // now get the actual page data
            fetch(pageRoot + "/page.html?t=" + Math.round(Date.now()/1000)).then(req => {
                req.text().then(pageHTML => {
                    var pageData = {
                        "manifest": pageManifest,
                        "html": pageHTML
                    }
                    callback(pageData)
                })
            })
        })
    })
}

function load_default(callback) {
    load_data("data/default/data.json", data => {
        callback(data);
    })
}

function load_all_non_page_specific_data(callback) {
    // This will load in all data, and return back a big "data" object which can then be referenced for page state
    set_rendering_notice("Downloading components...");
    load_components(components => {
        set_rendering_notice("Downloading manifest");
        load_pages_manifest(pages => {
            set_rendering_notice("Downloading default manifest");
            load_default(defaultPageManifest => {
                callback({
                    "components": components,
                    "pages": pages,
                    "default": defaultPageManifest
                })
            })
        })
    })
}

function load_all_data() {
    function load_page_iterative(pages_remaining, data, callback) {
        if (pages_remaining.length == 0) {
            callback(data);
            return;
        }
        var currentPage = pages_remaining.pop();
        set_rendering_notice("Downloading " + currentPage);
        load_page(currentPage, pageData => {
            data[currentPage] = pageData;
            load_page_iterative(pages_remaining, data, callback);
        })
    }

    function load_special_iterative(special_remaining, data, callback) {
        if (special_remaining.length == 0) {
            callback(data);
            return;
        }
        var current_special = special_remaining.pop();
        set_rendering_notice("Downloading " + current_special.target);
        fetch("data/special/" + current_special.path + "?t=" + Math.round(Date.now()/1000)).then(req => {
            req.text().then(special_text => {
                data[current_special.target] = special_text;
                load_special_iterative(special_remaining, data, callback);
            })
        })
    }

    load_all_non_page_specific_data(data => {
        window.web3D = data;
        load_special_iterative(data.pages.special_pages.slice(), {}, specialPages => {
            for (var i = 0; i < Object.keys(specialPages).length; i++) {
                document.getElementById(Object.keys(specialPages)[i]).innerHTML = specialPages[Object.keys(specialPages)[i]];
            }
            load_page_iterative(data.pages.pages.slice(), {}, loadedPages => {
                render_default();
                hide_rendering_notice();
                data["loadedPages"] = loadedPages;
                data["loadedSpecial"] = specialPages
                window.web3D = data;
                if (get_page_id() == "") {
                    change_page(window.web3D.pages.default);
                } else {
                    change_page(get_page_id());
                }
            })
        })
    })
}

function get_page_id() {
    return window.location.search.replace("?page=", "");
}

function modify_state(newSubPath) {
    var path = window.location.pathname.split("/");
    path[path.length - 1] = "?page=" + newSubPath;
    window.history.replaceState({ "path": newSubPath }, newSubPath, path.join('/'));
}

function change_page(pageId) {
    modify_state(pageId);
    render_page_from_id(get_page_id());
    loadFromDb();
}

function queryKeyLookup(query, callback) {
    load_json('backend/query.php?key=' + query, data => {
        callback(query, data);
    });
}

function loadFromDb() {
    var elementsToLoad = document.getElementsByClassName('db-load');
    for (var i = 0; i < elementsToLoad.length; i++) {
        var el = elementsToLoad[i];
        console.log(el.id);
        queryKeyLookup(el.id, (id, query) => {
            if (!query) {
                document.getElementById(id).innerHTML = "{" + id + "}";
                return;
            }
            console.log(query);
            document.getElementById(query.key).innerHTML = query.data;
            document.getElementById(query.key).classList.remove("placeholder-wave");
            document.getElementById(query.key).classList.remove("placeholder");
        })
    }
}

window.render_page_from_id = render_page_from_id;
load_all_data();