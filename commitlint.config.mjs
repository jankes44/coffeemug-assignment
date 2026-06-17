export default {
  extends: ['@commitlint/config-conventional'],
  ignores: [
    (message) => /^fix: Apply suggestions from code review(?:\n|$)/.test(message),
  ],
};
