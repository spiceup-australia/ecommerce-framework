package com.sdl.ecommerce.dxa.model;

import com.sdl.ecommerce.api.Query;
import com.sdl.ecommerce.api.QueryInputContributor;
import com.sdl.ecommerce.api.model.Breadcrumb;
import com.sdl.webapp.common.api.mapping.annotations.SemanticEntity;
import com.sdl.webapp.common.api.mapping.annotations.SemanticProperty;
import com.sdl.webapp.common.api.model.entity.AbstractEntityModel;

import java.util.List;

import static com.sdl.webapp.common.api.mapping.config.SemanticVocabulary.SDL_CORE;

/**
 * Facets Widget
 *
 * @author nic
 */
@SemanticEntity(entityName = "BreadcrumbWidget", vocabulary = SDL_CORE, prefix = "e", public_ = false)
public class BreadcrumbWidget extends AbstractEntityModel implements QueryInputContributor {

    @SemanticProperty("e:includeStoreLink")
    private boolean includeStoreLink = false;

    private List<Breadcrumb> breadcrumbs;
    private int totalItems;

    @Override
    public void contributeToQuery(Query query) {

        // TODO: How to handle this???
        //queryConfiguration.setShowStoreLink(includeStoreLink);
    }

    public List<Breadcrumb> getBreadcrumbs() {
        return breadcrumbs;
    }

    public void setBreadcrumbs(List<Breadcrumb> breadcrumbs) {
        this.breadcrumbs = breadcrumbs;
    }

    public int getTotalItems() {
        return totalItems;
    }

    public void setTotalItems(int totalItems) {
        this.totalItems = totalItems;
    }
}
