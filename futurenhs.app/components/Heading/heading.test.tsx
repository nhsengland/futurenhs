import * as React from 'react';
import { mount, shallow } from 'enzyme';

import { Heading } from '@components/index';

import { Props } from './interfaces';

const testProps: Props = {
    children: 'mock'
};

describe('Heading', () => {

    it('renders successfully with the default heading level when none passed in', () => {

        const props = Object.assign({}, testProps);
        const wrapper = shallow(<Heading {...props} />);

        expect(wrapper.find('h3').exists()).toBe(true);

    });

    it('renders the requested heading level when passed in', () => {

        const props = Object.assign({}, testProps, {
            level: 2
        });
        const wrapper = shallow(<Heading {...props} />);

        expect(wrapper.find('h2').exists()).toBe(true);

    });

    it('correctly clamps requested heading levels to within valid range', () => {

        const propsTooLow = Object.assign({}, testProps, {
            level: -5
        });

        const propsTooHigh = Object.assign({}, testProps, {
            level: 10
        });

        const wrapperTooLow = shallow(<Heading {...propsTooLow} />);
        const wrapperTooHigh = shallow(<Heading {...propsTooHigh} />);

        expect(wrapperTooLow.find('h1').exists()).toBe(true);
        expect(wrapperTooHigh.find('h6').exists()).toBe(true);

    });

    it('renders the title content passed in', () => {

        const props = Object.assign({}, testProps);
        const wrapper = mount(<Heading {...props} />);

        expect(wrapper.find('h3').text()).toEqual(testProps.children);

    });

});